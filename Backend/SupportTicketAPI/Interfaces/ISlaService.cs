using SupportTicketAPI.Models;

namespace SupportTicketAPI.Interfaces;

public interface ISlaService
{
    DateTime CalculateDeadline(TicketPriority priority, DateTime createdAt);
    bool IsBreached(DateTime slaDeadline);
    int HoursForPriority(TicketPriority priority);
}
