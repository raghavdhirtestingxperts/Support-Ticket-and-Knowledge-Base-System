namespace SupportTicketAPI.DTOs;


public record AddCommentDto(string Content);

public record CommentResponseDto(
    int      Id,
    string   Content,
    DateTime CreatedAt,
    int      UserId,
    string   UserName
);
