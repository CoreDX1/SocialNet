namespace SocialNet.Application.DTOs.Post;

public class PostDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? UserAvatarUrl { get; set; }
    public int LikesCount { get; set; }
    public int CommentCount { get; set; }
    public bool IsLikeByCurrentUser { get; set; }
    public DateTime CreatedAt { get; set; }

    // Constructor opcional para inicializar datos fácilmente
    public PostDto(
        Guid id,
        string content,
        string? imageUrl,
        Guid userId,
        string username,
        string? userAvatarUrl,
        int likesCount,
        int commentCount,
        bool isLikeByCurrentUser,
        DateTime createdAt
    )
    {
        Id = id;
        Content = content;
        ImageUrl = imageUrl;
        UserId = userId;
        Username = username;
        UserAvatarUrl = userAvatarUrl;
        LikesCount = likesCount;
        CommentCount = commentCount;
        IsLikeByCurrentUser = isLikeByCurrentUser;
        CreatedAt = createdAt;
    }

    // Constructor vacío requerido por algunos serializadores
    public PostDto() { }
}
