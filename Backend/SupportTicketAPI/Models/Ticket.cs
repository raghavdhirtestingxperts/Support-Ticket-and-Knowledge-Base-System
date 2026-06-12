namespace SupportTicketAPI.Models;

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime SlaDeadline { get; set; }
    public bool IsSlaBreached { get; set; } = false;

    public int CreatedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }

    public User CreatedBy { get; set; } = null!;
    public User? AssignedTo { get; set; }
    public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    public ICollection<TicketHistory> History { get; set; } = new List<TicketHistory>();
}

public enum TicketStatus
{
    Open,
    InProgress,
    PendingCustomer,
    Resolved,
    Closed
}

public enum TicketPriority
{
    Low,
    Medium,
    High,
    Critical
}
