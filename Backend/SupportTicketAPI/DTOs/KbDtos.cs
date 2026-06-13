namespace SupportTicketAPI.DTOs;


public record CreateArticleDto(
    string Title,
    string Body,
    string Tags
);

public record ArticleResponseDto(
    int       Id,
    string    Title,
    string    Body,
    string    Tags,
    DateTime  CreatedAt,
    DateTime? UpdatedAt,
    int       CreatedByUserId,
    string    CreatedByName
);
