using SocialNet.Application.DTOs.Comment;
using SocialNet.Application.DTOs.Common;

namespace SocialNet.Application.Interfaces;

public interface ICommentService
{
    Task<CommentDto> CreateAsync(
        Guid postId,
        Guid userId,
        CreateCommentRequest request,
        CancellationToken ct = default
    );
    Task<PagedResult<CommentDto>> GetByPostAsync(
        Guid postId,
        int page,
        int pageSize,
        CancellationToken ct = default
    );
    Task DeleteAsync(Guid commentId, Guid userId, CancellationToken ct = default);
}
