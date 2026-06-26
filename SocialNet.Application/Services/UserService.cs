using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SocialNet.Application.DTOs.Common;
using SocialNet.Application.DTOs.User;
using SocialNet.Application.Interfaces;

namespace SocialNet.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly DbContext _db;
    private readonly IFileStorageService _storage;

    public UserService(IUnitOfWork uow, IMapper mapper, DbContext db, IFileStorageService storage)
    {
        _uow = uow;
        _mapper = mapper;
        _db = db;
        _storage = storage;
    }

    public async Task<UserProfileDto> GetProfileAsync(
        string username, Guid currentUserId, CancellationToken ct = default)
    {
        var user = await _db.Set<Domain.Entities.User>()
            .FirstOrDefaultAsync(u => u.Username == username, ct)
            ?? throw new ApplicationException("Usuario no encontrado.");

        var postsCount = await _db.Set<Domain.Entities.Post>()
            .CountAsync(p => p.UserId == user.Id, ct);

        var followersCount = await _db.Set<Domain.Entities.Follow>()
            .CountAsync(f => f.FollowingId == user.Id, ct);

        var followingCount = await _db.Set<Domain.Entities.Follow>()
            .CountAsync(f => f.FollowerId == user.Id, ct);

        var isFollowedByCurrentUser = await _db.Set<Domain.Entities.Follow>()
            .AnyAsync(f => f.FollowerId == currentUserId && f.FollowingId == user.Id, ct);

        return new UserProfileDto(
            user.Id,
            user.Username,
            user.DisplayName,
            user.Bio,
            user.AvatarUrl,
            postsCount,
            followersCount,
            followingCount,
            isFollowedByCurrentUser
        );
    }

    public async Task<UserDto> UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var user = await _db.Set<Domain.Entities.User>()
            .FindAsync(new object[] { userId }, ct)
            ?? throw new ApplicationException("Usuario no encontrado.");

        if (request.DisplayName is not null)
            user.DisplayName = request.DisplayName;

        if (request.Bio is not null)
            user.Bio = request.Bio;

        await _uow.SaveChangesAsync(ct);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<string> UploadAvatarAsync(
        Guid userId, Stream fileStream, string fileName, CancellationToken ct = default)
    {
        var user = await _db.Set<Domain.Entities.User>()
            .FindAsync(new object[] { userId }, ct)
            ?? throw new ApplicationException("Usuario no encontrado.");

        if (!string.IsNullOrEmpty(user.AvatarUrl))
            _storage.DeleteFile(user.AvatarUrl);

        var url = await _storage.SaveFileAsync(fileStream, fileName, "avatars", ct);
        user.AvatarUrl = url;

        await _uow.SaveChangesAsync(ct);
        return url;
    }

    public async Task<PagedResult<UserDto>> SearchUsersAsync(
        string query, int page, int pageSize, CancellationToken ct = default)
    {
        var normalized = query.Trim().ToLower();

        var usersQuery = _db.Set<Domain.Entities.User>()
            .Where(u =>
                u.Username.ToLower().Contains(normalized) ||
                u.DisplayName.ToLower().Contains(normalized))
            .OrderBy(u => u.Username);

        return await PagedResult<UserDto>.CreateAsync(
            usersQuery.ProjectTo<UserDto>(_mapper.ConfigurationProvider),
            page, pageSize, ct);
    }
}
