using Microsoft.EntityFrameworkCore;
using SupportTicketAPI.Data;
using SupportTicketAPI.DTOs;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Services;

public interface IKbService
{
    Task<IEnumerable<ArticleResponseDto>> GetArticlesAsync(string? search);
    Task<ArticleResponseDto> CreateArticleAsync(CreateArticleDto dto, int agentUserId);
}

public class KbService : IKbService
{
    private readonly AppDbContext _db;

    public KbService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<ArticleResponseDto>> GetArticlesAsync(string? search)
    {
        var query = _db.KnowledgeBaseArticles
            .Include(a => a.CreatedBy)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(a =>
                a.Title.ToLower().Contains(term) ||
                a.Body.ToLower().Contains(term)  ||
                a.Tags.ToLower().Contains(term));
        }

        var articles = await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
        return articles.Select(MapToDto);
    }

    public async Task<ArticleResponseDto> CreateArticleAsync(CreateArticleDto dto, int agentUserId)
    {
        var article = new KnowledgeBaseArticle
        {
            Title           = dto.Title,
            Body            = dto.Body,
            Tags            = dto.Tags,
            CreatedAt       = DateTime.UtcNow,
            UpdatedAt       = null,
            CreatedByUserId = agentUserId
        };

        _db.KnowledgeBaseArticles.Add(article);
        await _db.SaveChangesAsync();

        await _db.Entry(article).Reference(a => a.CreatedBy).LoadAsync();
        return MapToDto(article);
    }

    private static ArticleResponseDto MapToDto(KnowledgeBaseArticle a) => new(
        a.Id, a.Title, a.Body, a.Tags,
        a.CreatedAt, a.UpdatedAt,
        a.CreatedByUserId, a.CreatedBy.Name
    );
}
