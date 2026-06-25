namespace SocialNet.Application.DTOs.User;

public record UserProfileDto(
    Guid Id,
    string Username,
    string DisplayName,
    string? Bio,
    string? AvatarUrl,
    int PostCout,
    int FollowersCount,
    int FollowingCount,
    bool IsFollowedByCurrentUser
);
