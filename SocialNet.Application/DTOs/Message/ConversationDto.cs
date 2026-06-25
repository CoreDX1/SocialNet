namespace SocialNet.Application.DTOs.Message;

public record ConversationDto(
    Guid OtherUserId,
    string OtherUsername,
    string? OtherAvatarUrl,
    MessageDto LastMessage,
    int UnreadCount
);
