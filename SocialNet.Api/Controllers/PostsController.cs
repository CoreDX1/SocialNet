using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNet.Application.DTOs.Post;
using SocialNet.Application.Interfaces;

namespace SocialNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly IPostService _posts;

    public PostsController(IPostService posts) => _posts = posts;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromForm] CreatePostRequest request,
        CancellationToken ct
    )
    {
        var result = await _posts.CreateAsync(UserId, request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var post = await _posts.GetByIdAsync(id, UserId, ct);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var result = await _posts.GetFeedAsync(UserId, page, pageSize, ct);
        return Ok(result);
    }

    [HttpGet("user/{username}")]
    public async Task<IActionResult> GetUserPosts(
        string username,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default
    )
    {
        var result = await _posts.GetUserPostsAsync(username, UserId, page, pageSize, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _posts.DeleteAsync(id, UserId, ct);
        return NoContent();
    }
}
