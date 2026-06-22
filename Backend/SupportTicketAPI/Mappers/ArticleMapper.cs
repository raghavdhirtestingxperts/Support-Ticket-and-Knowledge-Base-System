using SupportTicketAPI.DTOs;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Mappers;

/// <summary>
/// Pure mapping functions for KnowledgeBaseArticle → DTOs.
/// Extracted from KnowledgeBaseController to honour Single Responsibility.
/// </summary>
public static class ArticleMapper
{
    public static ArticleResponseDto ToResponse(KnowledgeBaseArticle a) => new(
        a.Id, a.Title, a.Body, a.Tags,
        a.CreatedAt, a.UpdatedAt,
        a.CreatedByUserId, a.CreatedBy.Name
    );
}
