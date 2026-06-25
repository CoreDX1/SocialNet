using SocialNet.Application.DTOs.Common;
using SocialNet.Application.DTOs.Post;

namespace SocialNet.Application.Interfaces;

public interface IPostService
{
    Task<PostDto> CreateAsync(Guid userId, CreatePostRequest request, CancellationToken ct = default);
    Task<PostDto?> GetByIdAsync(Guid postId, Guid currentUserId, CancellationToken ct = default);
    Task<PagedResult<PostDto>> GetFeedAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);
    Task<PagedResult<PostDto>> GetUserPostsAsync(string username, Guid currentUserId, int page, int pageSize, CancellationToken ct = default);
    Task DeleteAsync(Guid postId, Guid userId, CancellationToken ct = default);
}

