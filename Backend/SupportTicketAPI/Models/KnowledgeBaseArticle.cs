namespace SupportTicketAPI.Models;

public class KnowledgeBaseArticle
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public User CreatedBy { get; set; } = null!;
}
