namespace SocialNet.Application.DTOs.User;

public record UserDto(Guid Id, string Username, string DisplayName, string? Bio, string? AvatarUrl);
