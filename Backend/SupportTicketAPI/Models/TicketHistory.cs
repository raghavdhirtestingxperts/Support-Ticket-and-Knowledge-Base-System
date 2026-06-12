namespace SupportTicketAPI.Models;

public class TicketHistory
{
    public int Id { get; set; }
    public TicketStatus? OldStatus { get; set; }
    public TicketStatus NewStatus { get; set; }
    public string? Note { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow; 

    public int TicketId { get; set; }
    public int ChangedByUserId { get; set; }

    public Ticket Ticket { get; set; } = null!;
    public User ChangedBy { get; set; } = null!;
}
