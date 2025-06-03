using System.ComponentModel.DataAnnotations;

namespace DnkChangeHistoryService.Application.CommentHistories.Queries.History;

public class CommentHistoryQuery
{
    [Required]
    public Guid ReportId { get; set; }

    [Required]
    public string ContextId { get; set; } = string.Empty;

    [Required]
    public string Language { get; set; } = string.Empty;
}
