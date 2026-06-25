namespace SocialNet.Application.DTOs.Post;

public record PostDto(
    Guid Id,
    string Content,
    string? ImageUrl,
    Guid UserId,
    string Username,
    string? AvatarUrl,
    int LikesCount,
    int CommentCount,
    bool isLikeByCurrentUser,
    DateTime CreatedAt
);
