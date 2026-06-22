using System.ComponentModel.DataAnnotations;

namespace SupportTicketAPI.DTOs;


public record CreateArticleDto(
    [Required, MaxLength(200)]
    string Title,

    [Required, MaxLength(10000)]
    string Body,

    [MaxLength(500)]
    string Tags
);

public record UpdateArticleDto(
    [Required, MaxLength(200)]
    string Title,

    [Required, MaxLength(10000)]
    string Body,

    [MaxLength(500)]
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
