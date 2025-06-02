using DnkChangeHistoryService.Domain.ChangeHistoryManagement;
using DnkChangeHistoryService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Infrastructure.Repositories.CommentHistories.Queries;

[ExcludeFromCodeCoverage]
public class CommentHistoryQueryRepository(ChangeHistoryWorkContext context) : ICommentHistoryQueryRepository
{
    public async Task<IEnumerable<CommentHistory>> GetCommentHistories(Guid commentId, CancellationToken cancellationToken)
    {
        return await context.CommentHistories
            .Where(ch => ch.CommentId == commentId)
            .ToListAsync(cancellationToken);
    }
}
