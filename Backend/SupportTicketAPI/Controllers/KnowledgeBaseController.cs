using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketAPI.DTOs;
using SupportTicketAPI.Services;

namespace SupportTicketAPI.Controllers;

[ApiController]
[Route("api/kb")]
[Authorize]
public class KnowledgeBaseController : ControllerBase
{
    private readonly IKbService _kb;

    public KnowledgeBaseController(IKbService kb) => _kb = kb;

    private int UserId => int.Parse(User.FindFirstValue("UserId")!);

    [HttpGet("articles")]
    [ProducesResponseType(typeof(IEnumerable<ArticleResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetArticles([FromQuery] string? search)
    {
        var result = await _kb.GetArticlesAsync(search);
        return Ok(result);
    }

    [HttpPost("articles")]
    [Authorize(Roles = "Agent")]
    [ProducesResponseType(typeof(ArticleResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateArticle([FromBody] CreateArticleDto dto)
    {
        var result = await _kb.CreateArticleAsync(dto, UserId);
        return CreatedAtAction(nameof(GetArticles), new { }, result);
    }
}
