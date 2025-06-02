using DnkChangeHistoryService.Domain.ChangeHistory.CommentHistory;
using DnkChangeHistoryService.Domain.Helpers;

namespace DnkChangeHistoryService.Domain.ChangeHistoryManagement;

public class CommentHistory : IEntity
{
    public Guid Id { get; set; }

    public DateTime CreationDate { get; set; }

    public Guid UserId { get; set; }

    public Guid CommentId { get; set; }

    public CommentHistoryReason Reason { get; set; }
}