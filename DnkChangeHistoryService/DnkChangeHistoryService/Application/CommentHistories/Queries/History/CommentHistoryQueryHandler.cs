using DnkChangeHistoryService.Application.CommentHistories.Mapper;
using DnkChangeHistoryService.Domain.ChangeHistoryManagement;
using DnkChangeHistoryService.Helper;
using DnkChangeHistoryService.Infrastructure.Repositories.CommentHistories.Queries;

namespace DnkChangeHistoryService.Application.CommentHistories.Queries.History;

public class CommentHistoryQueryHandler(
    ILogger<CommentHistoryQueryHandler> logger,
    ICommentHistoryQueryRepository queryRepository,
    ICommentHistoryMapper mapper) : ICommentHistoryQueryHandler
{
    public async Task<List<CommentHistoryResponse>> Handle(CommentHistoryQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching comment history reportId: {ReportId}, contextId: {ContextId}, language: {Language}",
            request.ReportId, request.ContextId.SanitizeLogValue(), request.Language.SanitizeLogValue());

        IEnumerable<CommentHistory> commentHistories = await queryRepository.GetCommentHistories(request.ReportId, request.ContextId, cancellationToken);

        if (commentHistories == null)
        {
            logger.LogWarning("No comment history found for reportId: {ReportId}, contextId: {ContextId}", request.ReportId, request.ContextId.SanitizeLogValue());
            return new ();
        }

        return mapper.MapToResponse(commentHistories, request.Language);
    }
}
