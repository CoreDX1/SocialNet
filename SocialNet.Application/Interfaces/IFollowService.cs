using SocialNet.Application.DTOs.Common;
using SocialNet.Application.DTOs.User;

namespace SocialNet.Application.Interfaces;

public interface IFollowService
{
    Task<bool> ToggleFollowAsync(Guid followerId, Guid followingId, CancellationToken ct = default);
    Task<PagedResult<UserDto>> GetFollowersAsync(
        string username,
        int page,
        int pageSize,
        CancellationToken ct = default
    );
    Task<PagedResult<UserDto>> GetFollowingAsync(
        string username,
        int page,
        int pageSize,
        CancellationToken ct = default
    );
}
