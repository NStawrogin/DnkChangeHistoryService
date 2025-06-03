namespace DnkChangeHistoryService.Domain.ChangeHistory.CommentHistory;

public record CommentHistoryMessage(Guid ReportId, string ContextId, string DisclosureRequirementId, Guid CommentId, Guid UserId, DateTime Timestamp, CommentHistoryReason Reason);
