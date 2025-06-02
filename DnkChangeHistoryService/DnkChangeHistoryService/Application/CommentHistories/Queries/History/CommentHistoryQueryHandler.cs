using DnkChangeHistoryService.Application.CommentHistories.Mapper;
using DnkChangeHistoryService.Infrastructure.Repositories.CommentHistories.Queries;
using DnkChangeHistoryService.Domain.ChangeHistoryManagement;

namespace DnkChangeHistoryService.Application.CommentHistories.Queries.History;

public class CommentHistoryQueryHandler(
    ILogger<CommentHistoryQueryHandler> logger,
    ICommentHistoryQueryRepository queryHandler,
    ICommentHistoryMapper mapper) : ICommentHistoryQueryHandler
{
    public async Task<List<CommentHistoryResponse>> Handle(CommentHistoryQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling CommentHistoryQuery for commentId: {CommentId}", request.CommentId);

        IEnumerable<CommentHistory> commentHistories = await queryHandler.GetCommentHistories(request.CommentId, cancellationToken);

        if (commentHistories == null)
        {
            logger.LogWarning("No comment history found for commentId: {CommentId}", request.CommentId);
            return new ();
        }

        return mapper.MapToResponse(commentHistories, request.Language);
    }
}
