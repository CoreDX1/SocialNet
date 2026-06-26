using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SocialNet.Application.DTOs.Comment;
using SocialNet.Application.DTOs.Common;
using SocialNet.Application.Interfaces;
using SocialNet.Domain.Entities;

namespace SocialNet.Application.Services;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly DbContext _db;

    public CommentService(IUnitOfWork uow, IMapper mapper, DbContext db)
    {
        _uow = uow;
        _mapper = mapper;
        _db = db;
    }

    public async Task<CommentDto> CreateAsync(
        Guid postId,
        Guid userId,
        CreateCommentRequest request,
        CancellationToken ct = default
    )
    {
        var postExists = await _db.Set<Post>().AnyAsync(p => p.Id == postId, ct);
        if (!postExists)
            throw new ApplicationException("El post no existe.");

        var comment = new Comment
        {
            Content = request.Content,
            UserId = userId,
            PostId = postId,
        };

        _db.Set<Comment>().Add(comment);
        await _uow.SaveChangesAsync(ct);

        // Recargar con datos del usuario para el mapeo
        var result = await _db.Set<Comment>()
            .Include(c => c.User)
            .Where(c => c.Id == comment.Id)
            .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
            .FirstAsync(ct);

        return result;
    }

    public async Task<PagedResult<CommentDto>> GetByPostAsync(
        Guid postId,
        int page,
        int pageSize,
        CancellationToken ct = default
    )
    {
        var query = _db.Set<Comment>()
            .Include(c => c.User)
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt);

        return await PagedResult<CommentDto>.CreateAsync(
            query.ProjectTo<CommentDto>(_mapper.ConfigurationProvider),
            page,
            pageSize,
            ct
        );
    }

    public async Task DeleteAsync(Guid commentId, Guid userId, CancellationToken ct = default)
    {
        var comment =
            await _db.Set<Comment>().FindAsync(new object[] { commentId }, ct)
            ?? throw new ApplicationException("Comentario no encontrado.");

        if (comment.UserId != userId)
            throw new ApplicationException("No autorizado para eliminar este comentario.");

        _db.Set<Comment>().Remove(comment);
        await _uow.SaveChangesAsync(ct);
    }
}
