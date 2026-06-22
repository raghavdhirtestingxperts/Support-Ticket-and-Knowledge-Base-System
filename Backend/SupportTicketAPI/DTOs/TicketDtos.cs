using System.ComponentModel.DataAnnotations;

namespace SupportTicketAPI.DTOs;


public record CreateTicketDto(
    [Required, MaxLength(200)]
    string Title,

    [Required, MaxLength(5000)]
    string Description,

    [Required, RegularExpression("^(Low|Medium|High|Critical)$",
        ErrorMessage = "Priority must be Low, Medium, High, or Critical.")]
    string Priority   
);

public record AssignTicketDto(
    [Required]
    int AgentUserId
);

public record UpdateStatusDto(
    [Required, MaxLength(50)]
    string NewStatus, 

    [MaxLength(500)]
    string? Note     
);

public record TicketResponseDto(
    int      Id,
    string   Title,
    string   Description,
    string   Status,
    string   Priority,
    DateTime CreatedAt,
    DateTime SlaDeadline,
    bool     IsSlaBreached,
    int      CreatedByUserId,
    string   CreatedByName,
    int?     AssignedToUserId,
    string?  AssignedToName,
    IEnumerable<CommentResponseDto>      Comments,
    IEnumerable<TicketHistoryResponseDto> History
);

public record TicketListItemDto(
    int      Id,
    string   Title,
    string   Status,
    string   Priority,
    DateTime CreatedAt,
    DateTime SlaDeadline,
    bool     IsSlaBreached,
    string   CreatedByName,
    string?  AssignedToName
);

public record TicketHistoryResponseDto(
    int       Id,
    string?   OldStatus,
    string    NewStatus,
    string?   Note,
    DateTime  ChangedAt,
    string    ChangedByName
);
