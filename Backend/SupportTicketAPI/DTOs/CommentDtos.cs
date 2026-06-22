using System.ComponentModel.DataAnnotations;

namespace SupportTicketAPI.DTOs;


public record AddCommentDto(
    [Required, MaxLength(2000)]
    string Content
);

public record CommentResponseDto(
    int      Id,
    string   Content,
    DateTime CreatedAt,
    int      UserId,
    string   UserName
);
