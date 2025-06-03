using System.ComponentModel.DataAnnotations;

namespace DnkChangeHistoryService.Application.CommentHistories.Queries.History;

public class CommentHistoryQuery
{
    [Required]
    public Guid CommentId { get; set; }

    [Required]
    public string Language { get; set; } = string.Empty;
}
