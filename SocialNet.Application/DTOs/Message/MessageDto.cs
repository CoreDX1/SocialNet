namespace SocialNet.Application.DTOs.Message;

public record MessageDto(
    Guid Id,
    string Content,
    Guid SenderId,
    string SenderUsername,
    Guid ReceiverId,
    string ReceiverUsername,
    bool IsRead,
    DateTime CreatedAt
);
