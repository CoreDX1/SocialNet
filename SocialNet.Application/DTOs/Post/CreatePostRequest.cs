namespace SocialNet.Application.DTOs.Post;

public record CreatePostRequest(string Content, Stream? Image);
