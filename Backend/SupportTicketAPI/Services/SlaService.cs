using SupportTicketAPI.Interfaces;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Services;

// computes sla deadlines and breaches
public class SlaService : ISlaService
{
    private static readonly Dictionary<TicketPriority, int> SlaHours = new()
    {
        { TicketPriority.Critical, 2  },
        { TicketPriority.High,     8  },
        { TicketPriority.Medium,   24 },
        { TicketPriority.Low,      72 }
    };

    public int HoursForPriority(TicketPriority priority)
        => SlaHours[priority];

    public DateTime CalculateDeadline(TicketPriority priority, DateTime createdAt)
        => createdAt.AddHours(HoursForPriority(priority));

    public bool IsBreached(DateTime slaDeadline)
        => DateTime.UtcNow > slaDeadline;
}
