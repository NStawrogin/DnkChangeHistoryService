namespace DnkChangeHistoryService.Application.CommentHistories.Queries.History;

public class CommentHistoryResponse
{
    public DateTime CreationDate { get; set; }

    public Guid UserId { get; set; }

    public Guid CommentId { get; set; }

    public string? Event { get; set; }
}
