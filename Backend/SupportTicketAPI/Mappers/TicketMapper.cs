using SupportTicketAPI.DTOs;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Mappers;

/// <summary>
/// Pure mapping functions for Ticket → DTOs.
/// Extracted from TicketsController to honour Single Responsibility.
/// </summary>
public static class TicketMapper
{
    public static TicketResponseDto ToResponse(Ticket t) => new(
        t.Id,
        t.Title,
        t.Description,
        t.Status.ToString(),
        t.Priority.ToString(),
        t.CreatedAt,
        t.SlaDeadline,
        t.IsSlaBreached,
        t.CreatedByUserId,
        t.CreatedBy.Name,
        t.AssignedToUserId,
        t.AssignedTo?.Name,
        t.Comments.OrderBy(c => c.CreatedAt).Select(c =>
            new CommentResponseDto(c.Id, c.Content, c.CreatedAt, c.UserId, c.User.Name)),
        t.History.OrderBy(h => h.ChangedAt).Select(h =>
            new TicketHistoryResponseDto(
                h.Id,
                h.OldStatus?.ToString(),
                h.NewStatus.ToString(),
                h.Note,
                h.ChangedAt,
                h.ChangedBy.Name))
    );

    public static TicketListItemDto ToListItem(Ticket t) => new(
        t.Id, t.Title, t.Status.ToString(), t.Priority.ToString(),
        t.CreatedAt, t.SlaDeadline, t.IsSlaBreached,
        t.CreatedBy.Name, t.AssignedTo?.Name
    );
}
