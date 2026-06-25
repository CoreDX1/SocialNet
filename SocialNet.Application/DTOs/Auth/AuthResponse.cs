namespace SocialNet.Application.DTOs.Auth;

public record AuthResponse(Guid UserId, string Username, string Token, DateTime Expiration);
