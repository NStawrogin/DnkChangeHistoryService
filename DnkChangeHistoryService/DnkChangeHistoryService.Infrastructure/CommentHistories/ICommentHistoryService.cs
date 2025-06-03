using DnkChangeHistoryService.Domain.ChangeHistory.CommentHistory;

namespace DnkChangeHistoryService.Infrastructure.CommentHistories;

public interface ICommentHistoryService
{
    Task HandleCommentHistoryAsync(CommentHistoryMessage message, CancellationToken cancellationToken);
}
