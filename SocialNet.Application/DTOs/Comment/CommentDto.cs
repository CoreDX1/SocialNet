namespace SocialNet.Application.DTOs.Comment;

public record CommentDto(
    Guid Id,
    string Content,
    Guid UserId,
    string Username,
    string? UserAvatarUrl,
    DateTime CreatedAt
);
