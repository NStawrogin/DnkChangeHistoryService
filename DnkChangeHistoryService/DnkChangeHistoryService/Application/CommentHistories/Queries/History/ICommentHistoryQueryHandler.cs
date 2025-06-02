namespace DnkChangeHistoryService.Application.CommentHistories.Queries.History;

public interface ICommentHistoryQueryHandler
{
    Task<List<CommentHistoryResponse>> Handle(CommentHistoryQuery request, CancellationToken cancellationToken);
}
