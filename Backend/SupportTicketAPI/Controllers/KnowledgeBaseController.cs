using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SupportTicketAPI.Constants;
using SupportTicketAPI.DTOs;
using SupportTicketAPI.Interfaces;
using SupportTicketAPI.Mappers;

namespace SupportTicketAPI.Controllers;

[ApiController]
[Route("api/kb")]
[Authorize]
[EnableRateLimiting("api")]
public class KnowledgeBaseController : ControllerBase
{
    private readonly IKbService _kb;

    public KnowledgeBaseController(IKbService kb) => _kb = kb;

    private int    UserId => int.Parse(User.FindFirstValue("UserId")!);
    private string Role   => User.FindFirstValue(ClaimTypes.Role)!;

    [HttpGet("articles")]
    [ProducesResponseType(typeof(IEnumerable<ArticleResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetArticles([FromQuery] string? search)
    {
        var articles = await _kb.GetArticlesAsync(search);
        return Ok(articles.Select(ArticleMapper.ToResponse));
    }

    [HttpPost("articles")]
    [Authorize(Roles = $"{RoleNames.Agent},{RoleNames.Supervisor}")]
    [ProducesResponseType(typeof(ArticleResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateArticle([FromBody] CreateArticleDto dto)
    {
        var article = await _kb.CreateArticleAsync(dto.Title, dto.Body, dto.Tags, UserId);
        return CreatedAtAction(nameof(GetArticles), new { }, ArticleMapper.ToResponse(article));
    }

    [HttpPut("articles/{id:int}")]
    [Authorize(Roles = $"{RoleNames.Agent},{RoleNames.Supervisor}")]
    [ProducesResponseType(typeof(ArticleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateArticle(int id, [FromBody] UpdateArticleDto dto)
    {
        try
        {
            var article = await _kb.UpdateArticleAsync(id, dto.Title, dto.Body, dto.Tags, UserId, Role);
            if (article is null) return NotFound(new { message = "Article not found." });
            return Ok(ArticleMapper.ToResponse(article));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    [HttpDelete("articles/{id:int}")]
    [Authorize(Roles = $"{RoleNames.Agent},{RoleNames.Supervisor}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        try
        {
            var deleted = await _kb.DeleteArticleAsync(id, UserId, Role);
            if (!deleted) return NotFound(new { message = "Article not found." });
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }
}
