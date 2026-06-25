using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SocialNet.Application.DTOs.Common;
using SocialNet.Application.DTOs.User;
using SocialNet.Application.Interfaces;
using SocialNet.Domain.Entities;

namespace SocialNet.Application.Services;

public class FollowService : IFollowService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly DbContext _db;

    public FollowService(IUnitOfWork uow, IMapper mapper, DbContext db)
    {
        _uow = uow;
        _mapper = mapper;
        _db = db;
    }

    public async Task<bool> ToggleFollowAsync(
        Guid followerId, Guid followingId, CancellationToken ct = default)
    {
        if (followerId == followingId)
            throw new ApplicationException("No puedes seguirte a ti mismo.");

        var targetExists = await _db.Set<User>().AnyAsync(u => u.Id == followingId, ct);
        if (!targetExists)
            throw new ApplicationException("El usuario no existe.");

        var existingFollow = await _db.Set<Follow>()
            .FirstOrDefaultAsync(
                f => f.FollowerId == followerId && f.FollowingId == followingId, ct);

        if (existingFollow is not null)
        {
            // Dejar de seguir
            _db.Set<Follow>().Remove(existingFollow);
            await _uow.SaveChangesAsync(ct);
            return false;
        }
        else
        {
            // Seguir
            var follow = new Follow
            {
                FollowerId = followerId,
                FollowingId = followingId
            };

            _db.Set<Follow>().Add(follow);
            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }

    public async Task<PagedResult<UserDto>> GetFollowersAsync(
        string username, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.Set<Follow>()
            .Include(f => f.Follower)
            .Where(f => f.Following.Username == username)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => f.Follower);

        return await PagedResult<UserDto>.CreateAsync(
            query.ProjectTo<UserDto>(_mapper.ConfigurationProvider),
            page, pageSize, ct);
    }

    public async Task<PagedResult<UserDto>> GetFollowingAsync(
        string username, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.Set<Follow>()
            .Include(f => f.Following)
            .Where(f => f.Follower.Username == username)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => f.Following);

        return await PagedResult<UserDto>.CreateAsync(
            query.ProjectTo<UserDto>(_mapper.ConfigurationProvider),
            page, pageSize, ct);
    }
}
