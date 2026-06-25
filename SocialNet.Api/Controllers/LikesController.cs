using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNet.Application.Interfaces;

namespace SocialNet.Api.Controllers;

[ApiController]
[Route("api/posts/{postId:guid}/like")]
[Authorize]
public class LikesController : ControllerBase
{
    private readonly ILikeService _likes;

    public LikesController(ILikeService likes) => _likes = likes;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Toggle(Guid postId, CancellationToken ct)
    {
        var isLiked = await _likes.ToggleLikeAsync(postId, UserId, ct);
        return Ok(new { isLiked });
    }
}
