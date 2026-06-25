using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNet.Application.DTOs.Message;
using SocialNet.Application.Interfaces;

namespace SocialNet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messages;

    public MessagesController(IMessageService messages) => _messages = messages;

    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> Send(SendMessageRequest request, CancellationToken ct)
    {
        var result = await _messages.SendAsync(UserId, request, ct);
        return Ok(result);
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        return Ok(await _messages.GetConversationsAsync(UserId, page, pageSize, ct));
    }

    [HttpGet("conversations/{otherUserId:guid}")]
    public async Task<IActionResult> GetConversation(Guid otherUserId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
    {
        return Ok(await _messages.GetConversationAsync(UserId, otherUserId, page, pageSize, ct));
    }

    [HttpPut("conversations/{otherUserId:guid}/read")]
    public async Task<IActionResult> MarkAsRead(Guid otherUserId, CancellationToken ct)
    {
        await _messages.MarkAsReadAsync(UserId, otherUserId, ct);
        return NoContent();
    }
}
