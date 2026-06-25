using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNet.Application.DTOs.Comment;
using SocialNet.Application.Interfaces;

namespace SocialNet.Api.Controllers;

[ApiController]
[Route("api/posts/{postId:guid}/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _comments;

    public CommentsController(ICommentService comments) => _comments = comments;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Create(
        Guid postId, CreateCommentRequest request, CancellationToken ct)
    {
        var result = await _comments.CreateAsync(postId, UserId, request, ct);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetByPost(
        Guid postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _comments.GetByPostAsync(postId, page, pageSize, ct);
        return Ok(result);
    }

    [HttpDelete("{commentId:guid}")]
    public async Task<IActionResult> Delete(Guid commentId, CancellationToken ct)
    {
        await _comments.DeleteAsync(commentId, UserId, ct);
        return NoContent();
    }
}
