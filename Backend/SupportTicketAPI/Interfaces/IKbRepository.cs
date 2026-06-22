using SupportTicketAPI.Models;

namespace SupportTicketAPI.Interfaces;

// kb article data access
public interface IKbRepository
{
    // get all articles with optional filter
    Task<List<KnowledgeBaseArticle>> GetAllAsync(string? search);

    // get article by id
    Task<KnowledgeBaseArticle?> GetByIdAsync(int id);

    // add new article
    void Add(KnowledgeBaseArticle article);

    // delete article
    void Delete(KnowledgeBaseArticle article);

    // load creator details
    Task LoadCreatedByAsync(KnowledgeBaseArticle article);

    // save changes
    Task SaveAsync();
}
