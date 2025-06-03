namespace DnkChangeHistoryService.Domain.ChangeHistory.CommentHistory;

public record CommentHistoryMessage(Guid CommentId, Guid UserId, DateTime Timestamp, CommentHistoryReason Reason);
