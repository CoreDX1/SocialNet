namespace SocialNet.Application.DTOs.Message;

public record SendMessageRequest(Guid ReceiverId, string Content);
