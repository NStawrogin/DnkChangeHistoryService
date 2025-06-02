using DnkChangeHistoryService.Application.CommentHistories.Queries.History;
using DnkChangeHistoryService.Domain.ChangeHistoryManagement;

namespace DnkChangeHistoryService.Application.CommentHistories.Mapper;

public interface ICommentHistoryMapper
{
    List<CommentHistoryResponse> MapToResponse(IEnumerable<CommentHistory> commentHistories, string language);
}
