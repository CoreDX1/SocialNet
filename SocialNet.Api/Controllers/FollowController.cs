using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNet.Application.Interfaces;
using SocialNet.Domain.Entities;

namespace SocialNet.Api.Controllers;

[ApiController]
[Route("api/users/{username}/follow")]
[Authorize]
public class FollowController : ControllerBase
{
    private readonly IFollowService _follows;
    private readonly UserManager<User> _userManager;

    public FollowController(IFollowService follows, UserManager<User> userManager)
    {
        _follows = follows;
        _userManager = userManager;
    }

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> ToggleFollow(string username, CancellationToken ct)
    {
        var followingUser = await _userManager.FindByNameAsync(username);
        if (followingUser == null) return NotFound();
        
        var result = await _follows.ToggleFollowAsync(UserId, followingUser.Id, ct);
        return Ok(new { isFollowing = result });
    }

    [HttpGet("followers")]
    public async Task<IActionResult> GetFollowers(string username, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        return Ok(await _follows.GetFollowersAsync(username, page, pageSize, ct));
    }

    [HttpGet("following")]
    public async Task<IActionResult> GetFollowing(string username, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        return Ok(await _follows.GetFollowingAsync(username, page, pageSize, ct));
    }
}
