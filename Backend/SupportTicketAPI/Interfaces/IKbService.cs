using SupportTicketAPI.Models;

namespace SupportTicketAPI.Interfaces;

// kb service contract
public interface IKbService
{
    Task<IEnumerable<KnowledgeBaseArticle>> GetArticlesAsync(string? search);
    Task<KnowledgeBaseArticle> CreateArticleAsync(string title, string body, string tags, int agentUserId);

    // update article (author or supervisor only)
    Task<KnowledgeBaseArticle?> UpdateArticleAsync(int id, string title, string body, string tags, int requestingUserId, string role);

    // delete article (author or supervisor only)
    Task<bool> DeleteArticleAsync(int id, int requestingUserId, string role);
}
