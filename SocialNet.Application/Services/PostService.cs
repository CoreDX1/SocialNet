using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SocialNet.Application.DTOs.Common;
using SocialNet.Application.DTOs.Post;
using SocialNet.Application.Interfaces;
using SocialNet.Domain.Entities;

namespace SocialNet.Application.Services;

public class PostService : IPostService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly DbContext _db;
    private readonly IFileStorageService _storage;

    public PostService(IUnitOfWork uow, IMapper mapper, DbContext db, IFileStorageService storage)
    {
        _uow = uow;
        _mapper = mapper;
        _db = db;
        _storage = storage;
    }

    public async Task<PostDto> CreateAsync(
        Guid userId,
        CreatePostRequest request,
        CancellationToken ct = default
    )
    {
        string? imageUrl = null;

        if (request.Image is not null)
        {
            imageUrl = await _storage.SaveFileAsync(request.Image, "image.jpg", "posts", ct);
        }

        var post = new Post
        {
            Content = request.Content,
            ImageUrl = imageUrl,
            UserId = userId,
        };

        _db.Set<Post>().Add(post);
        await _uow.SaveChangesAsync(ct);

        return (await GetByIdAsync(post.Id, userId, ct))!;
    }

    public async Task<PostDto?> GetByIdAsync(
        Guid postId,
        Guid currentUserId,
        CancellationToken ct = default
    )
    {
        IQueryable<Post> query = _db.Set<Post>().Where(p => p.Id == postId);

        var projectQuery = query.Select(p => new PostDto{
            Id = p.Id,
            Content = p.Content,
            ImageUrl = p.ImageUrl,
            UserId = p.UserId,
            Username = p.User.Username,
            UserAvatarUrl = p.User.AvatarUrl,
            LikesCount = p.Likes.Count,
            CommentCount = p.Comments.Count,
            CreatedAt = p.CreatedAt
        });

        // return await _db.Set<Post>()
        //     .Where(p => p.Id == postId)
        //     .ProjectTo<PostDto>(_mapper.ConfigurationProvider, new { currentUserId })
        //     .FirstOrDefaultAsync(ct);

        return await projectQuery.FirstOrDefaultAsync(ct);
    }

    public async Task<PagedResult<PostDto>> GetFeedAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken ct = default
    )
    {
        // Obtenemos los IDs de los seguidos
        var followingIds = await _db.Set<Follow>()
            .Where(f => f.FollowerId == userId)
            .Select(f => f.FollowingId)
            .ToListAsync(ct);

        followingIds.Add(userId); // incluir posts propios

        // Consulta la base de publicación
        var query = _db.Set<Post>()
            .Where(p => followingIds.Contains(p.UserId))
            .OrderByDescending(p => p.CreatedAt);

        var projectQuery = query.Select(p => new PostDto
        {
            Id = p.Id,
            Content = p.Content,
            ImageUrl = p.ImageUrl,
            UserId = p.UserId,
            Username = p.User.Username,
            UserAvatarUrl = p.User.AvatarUrl,
            LikesCount = p.Likes.Count,
            CommentCount = p.Comments.Count,
            IsLikeByCurrentUser = p.Likes.Any(l => l.UserId == userId),
            CreatedAt = p.CreatedAt
        });

        return await PagedResult<PostDto>.CreateAsync(projectQuery, page, pageSize, ct);
    }

    public async Task<PagedResult<PostDto>> GetUserPostsAsync(
        string username,
        Guid currentUserId,
        int page,
        int pageSize,
        CancellationToken ct = default
    )
    {
        var query = _db.Set<Post>()
            .Include(p => p.User)
            .Where(p => p.User.Username == username)
            .OrderByDescending(p => p.CreatedAt);

        return await PagedResult<PostDto>.CreateAsync(
            query.ProjectTo<PostDto>(_mapper.ConfigurationProvider, new { currentUserId }),
            page,
            pageSize,
            ct
        );
    }

    public async Task DeleteAsync(Guid postId, Guid userId, CancellationToken ct = default)
    {
        var post =
            await _db.Set<Post>().FindAsync(new object[] { postId }, ct)
            ?? throw new ApplicationException("Post no encontrado.");

        if (post.UserId != userId)
            throw new ApplicationException("No autorizado.");

        _db.Set<Post>().Remove(post);
        await _uow.SaveChangesAsync(ct);
    }
}
