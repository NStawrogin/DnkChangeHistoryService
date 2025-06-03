using DnkChangeHistoryService.Domain.ChangeHistoryManagement;

namespace DnkChangeHistoryService.Infrastructure.Repositories.CommentHistories.Queries;

public interface ICommentHistoryQueryRepository
{
    Task<IEnumerable<CommentHistory>> GetCommentHistories(Guid reportId, string contextId, CancellationToken cancellationToken);
}
