namespace DnkChangeHistoryService.Domain.ChangeHistory;

public record CommentHistoryMessage(DateTime DeletionTimestamp, string Reason);
