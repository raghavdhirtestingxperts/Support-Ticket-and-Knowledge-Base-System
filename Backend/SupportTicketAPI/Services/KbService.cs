using SupportTicketAPI.Constants;
using SupportTicketAPI.Interfaces;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Services;

// knowledge base logic implementation
public class KbService : IKbService
{
    private readonly IKbRepository _kb;

    public KbService(IKbRepository kb) => _kb = kb;

    public async Task<IEnumerable<KnowledgeBaseArticle>> GetArticlesAsync(string? search)
        => await _kb.GetAllAsync(search);

    public async Task<KnowledgeBaseArticle> CreateArticleAsync(
        string title, string body, string tags, int agentUserId)
    {
        var article = new KnowledgeBaseArticle
        {
            Title           = title,
            Body            = body,
            Tags            = tags,
            CreatedAt       = DateTime.UtcNow,
            UpdatedAt       = null,
            CreatedByUserId = agentUserId
        };

        _kb.Add(article);
        await _kb.SaveAsync();

        // load author relation for output dto
        await _kb.LoadCreatedByAsync(article);
        return article;
    }

    public async Task<KnowledgeBaseArticle?> UpdateArticleAsync(
        int id, string title, string body, string tags, int requestingUserId, string role)
    {
        var article = await _kb.GetByIdAsync(id);
        if (article is null) return null;

        // only author or supervisor can edit
        if (role != RoleNames.Supervisor && article.CreatedByUserId != requestingUserId)
            throw new UnauthorizedAccessException("You can only edit articles you created.");

        article.Title     = title;
        article.Body      = body;
        article.Tags      = tags;
        article.UpdatedAt = DateTime.UtcNow;

        await _kb.SaveAsync();
        return article;
    }

    public async Task<bool> DeleteArticleAsync(int id, int requestingUserId, string role)
    {
        var article = await _kb.GetByIdAsync(id);
        if (article is null) return false;

        // only author or supervisor can delete
        if (role != RoleNames.Supervisor && article.CreatedByUserId != requestingUserId)
            throw new UnauthorizedAccessException("You can only delete articles you created.");

        _kb.Delete(article);
        await _kb.SaveAsync();
        return true;
    }
}
