using SocialNet.Application.DTOs.Common;
using SocialNet.Application.DTOs.Message;

namespace SocialNet.Application.Interfaces;

public interface IMessageService
{
    Task<MessageDto> SendAsync(
        Guid senderId,
        SendMessageRequest request,
        CancellationToken ct = default
    );
    Task<PagedResult<ConversationDto>> GetConversationsAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken ct = default
    );
    Task<PagedResult<MessageDto>> GetConversationAsync(
        Guid userId,
        Guid otherUserId,
        int page,
        int pageSize,
        CancellationToken ct = default
    );
    Task MarkAsReadAsync(Guid userId, Guid otherUserId, CancellationToken ct = default);
}
