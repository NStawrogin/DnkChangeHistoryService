using DnkChangeHistoryService.Domain.ChangeHistory.CommentHistory;
using DnkChangeHistoryService.Domain.Helpers;

namespace DnkChangeHistoryService.Domain.ChangeHistoryManagement;

public class CommentHistory : IEntity
{
    public Guid Id { get; set; }

    public Guid ReportId { get; set; }

    public required string ContextId { get; set; }

    public required string DisclosureRequirementId { get; set; }

    public DateTime CreationDate { get; set; }

    public Guid UserId { get; set; }

    public Guid CommentId { get; set; }

    public CommentHistoryReason Reason { get; set; }
}