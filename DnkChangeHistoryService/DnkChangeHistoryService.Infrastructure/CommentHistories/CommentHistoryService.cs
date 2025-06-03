using DnkChangeHistoryService.Domain.ChangeHistory.CommentHistory;
using DnkChangeHistoryService.Infrastructure.Repositories.CommentHistories.Commands;

namespace DnkChangeHistoryService.Infrastructure.CommentHistories;

public class CommentHistoryService(ICommentHistoryCommandRepository commentHistoryCommandRepository) : ICommentHistoryService
{
    private readonly ICommentHistoryCommandRepository commentHistoryCommandRepository = commentHistoryCommandRepository;

    public async Task HandleCommentHistoryAsync(CommentHistoryMessage message, CancellationToken cancellationToken)
    {
        switch (message.Reason)
        {
            case CommentHistoryReason.Created:
                await commentHistoryCommandRepository.CreateCommentEntry(message, cancellationToken);
                break;

            default:
                break;
        }
    }
}
