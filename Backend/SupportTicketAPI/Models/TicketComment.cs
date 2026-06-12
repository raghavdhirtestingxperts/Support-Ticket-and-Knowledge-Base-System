namespace SupportTicketAPI.Models;

public class TicketComment
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int TicketId { get; set; }
    public int UserId { get; set; }

    public Ticket Ticket { get; set; } = null!;
    public User User { get; set; } = null!;
}
