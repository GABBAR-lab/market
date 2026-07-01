using System.Security.Claims;
using ChatService.Application.DTOs;
using ChatService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Api.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatAppService _chat;

    public ChatController(IChatAppService chat) => _chat = chat;

    [HttpGet("conversations")]
    public async Task<IActionResult> GetConversations()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var result = await _chat.GetMyConversationsAsync(userId);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost("conversations")]
    public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var result = await _chat.StartOrGetConversationAsync(userId, request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpGet("conversations/{conversationId:guid}/messages")]
    public async Task<IActionResult> GetMessages(Guid conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var result = await _chat.GetMessagesAsync(userId, conversationId, page, pageSize);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost("conversations/{conversationId:guid}/messages")]
    public async Task<IActionResult> SendMessage(Guid conversationId, [FromBody] SendMessageRequest request)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var result = await _chat.SendMessageAsync(userId, conversationId, request);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = default;
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(claim, out userId);
    }
}

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "healthy", service = "ChatService", timestamp = DateTime.UtcNow });
}
