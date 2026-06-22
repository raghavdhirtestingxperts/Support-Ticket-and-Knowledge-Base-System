using Microsoft.EntityFrameworkCore;
using SupportTicketAPI.Data;
using SupportTicketAPI.Interfaces;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Repositories;

// kb queries implementation
public class KbRepository : IKbRepository
{
    private readonly AppDbContext _db;

    public KbRepository(AppDbContext db) => _db = db;

    public Task<List<KnowledgeBaseArticle>> GetAllAsync(string? search)
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

        return query.OrderByDescending(a => a.CreatedAt).ToListAsync();
    }

    public Task<KnowledgeBaseArticle?> GetByIdAsync(int id)
        => _db.KnowledgeBaseArticles
            .Include(a => a.CreatedBy)
            .FirstOrDefaultAsync(a => a.Id == id);

    public void Add(KnowledgeBaseArticle article)
        => _db.KnowledgeBaseArticles.Add(article);

    public void Delete(KnowledgeBaseArticle article)
        => _db.KnowledgeBaseArticles.Remove(article);

    public Task LoadCreatedByAsync(KnowledgeBaseArticle article)
        => _db.Entry(article).Reference(a => a.CreatedBy).LoadAsync();

    public Task SaveAsync()
        => _db.SaveChangesAsync();
}
