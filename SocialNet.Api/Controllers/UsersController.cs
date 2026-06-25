using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNet.Application.DTOs.User;
using SocialNet.Application.Interfaces;

namespace SocialNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users) => _users = users;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("{username}")]
    public async Task<IActionResult> GetProfile(string username, CancellationToken ct)
    {
        var profile = await _users.GetProfileAsync(username, UserId, ct);
        return Ok(profile);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request, CancellationToken ct)
    {
        var result = await _users.UpdateProfileAsync(UserId, request, ct);
        return Ok(result);
    }

    [HttpPost("me/avatar")]
    public async Task<IActionResult> UploadAvatar(IFormFile file, CancellationToken ct)
    {
        using var stream = file.OpenReadStream();
        var url = await _users.UploadAvatarAsync(UserId, stream, file.FileName, ct);
        return Ok(new { avatarUrl = url });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _users.SearchUsersAsync(q, page, pageSize, ct);
        return Ok(result);
    }
}
