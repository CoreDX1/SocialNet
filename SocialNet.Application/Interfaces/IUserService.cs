using SocialNet.Application.DTOs.Common;
using SocialNet.Application.DTOs.User;

namespace SocialNet.Application.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetProfileAsync(
        string username,
        Guid currentUserId,
        CancellationToken ct = default
    );
    Task<UserDto> UpdateProfileAsync(
        Guid userId,
        UpdateProfileRequest request,
        CancellationToken ct = default
    );
    Task<string> UploadAvatarAsync(
        Guid userId,
        Stream fileStream,
        string fileName,
        CancellationToken ct = default
    );
    Task<PagedResult<UserDto>> SearchUsersAsync(
        string query,
        int page,
        int pageSize,
        CancellationToken ct = default
    );
}
