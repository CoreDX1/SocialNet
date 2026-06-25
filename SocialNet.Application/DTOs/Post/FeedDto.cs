namespace SocialNet.Application.DTOs.Post;

using SocialNet.Application.DTOs.Common;

public record FeedDto(PagedResult<PostDto> Posts);
