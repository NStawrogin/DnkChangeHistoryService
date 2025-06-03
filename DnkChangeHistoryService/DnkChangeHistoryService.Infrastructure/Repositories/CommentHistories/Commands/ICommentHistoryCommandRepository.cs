using DnkChangeHistoryService.Domain.ChangeHistory.CommentHistory;

namespace DnkChangeHistoryService.Infrastructure.Repositories.CommentHistories.Commands;

public interface ICommentHistoryCommandRepository
{
    Task CreateCommentEntry(CommentHistoryMessage message, CancellationToken cancellationToken);
}
