using DnkChangeHistoryService.Domain.ChangeHistory.CommentHistory;
using DnkChangeHistoryService.Domain.ChangeHistoryManagement;
using DnkChangeHistoryService.Infrastructure.Context;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Infrastructure.Repositories.CommentHistories.Commands;

[ExcludeFromCodeCoverage]
public class CommentHistoryCommandRepository(ChangeHistoryWorkContext context) : ICommentHistoryCommandRepository
{
    private readonly ChangeHistoryWorkContext context = context;

    public async Task CreateCommentEntry(CommentHistoryMessage message, CancellationToken cancellationToken)
    {
        CommentHistory entry = new()
        {
            Id = Guid.NewGuid(),
            CommentId = message.CommentId,
            UserId = message.UserId,
            CreationDate = message.Timestamp,
            Reason = message.Reason
        };

        this.context.CommentHistories.Add(entry);

        await this.context.SaveChangesAsync(cancellationToken);
    }
}
