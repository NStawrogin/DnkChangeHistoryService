using DnkChangeHistoryService.Domain.ChangeHistoryManagement;
using DnkChangeHistoryService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Infrastructure.Repositories.CommentHistories.Queries;

[ExcludeFromCodeCoverage]
public class CommentHistoryQueryRepository(ChangeHistoryWorkContext context) : ICommentHistoryQueryRepository
{
    public async Task<IEnumerable<CommentHistory>> GetCommentHistories(Guid reportId, string contextId, CancellationToken cancellationToken)
    {
        return await context.CommentHistories
            .Where(ch => ch.ReportId == reportId && ch.ContextId == contextId)
            .ToListAsync(cancellationToken);
    }
}
