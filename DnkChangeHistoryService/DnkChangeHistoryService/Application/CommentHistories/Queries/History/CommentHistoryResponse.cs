namespace DnkChangeHistoryService.Application.CommentHistories.Queries.History;

public class CommentHistoryResponse
{
    public Guid CommentId { get; set; }

    public DateTime CreationDate { get; set; }

    public Guid UserId { get; set; }

    public string? ContextId { get; set; }

    public string? DisclosureRequirementId { get; set; }

    public string? Event { get; set; }
}
