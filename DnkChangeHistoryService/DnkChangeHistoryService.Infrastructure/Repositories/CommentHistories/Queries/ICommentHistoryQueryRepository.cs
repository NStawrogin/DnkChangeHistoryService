using DnkChangeHistoryService.Domain.ChangeHistoryManagement;

namespace DnkChangeHistoryService.Infrastructure.Repositories.CommentHistories.Queries;

public interface ICommentHistoryQueryRepository
{
    Task<IEnumerable<CommentHistory>> GetCommentHistories(Guid commentId, CancellationToken cancellationToken);
}
