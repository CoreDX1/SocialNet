namespace SocialNet.Application.Interfaces;

public interface ILikeService
{
    Task<bool> ToggleLikeAsync(Guid postId, Guid userId, CancellationToken ct = default);
}
