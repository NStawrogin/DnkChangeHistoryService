using DnkChangeHistoryService.Application.CommentHistories.Queries.History;
using DnkChangeHistoryService.Domain.ChangeHistory.CommentHistory;
using DnkChangeHistoryService.Domain.ChangeHistoryManagement;

namespace DnkChangeHistoryService.Application.CommentHistories.Mapper;

public class CommentHistoryMapper : ICommentHistoryMapper
{
    public List<CommentHistoryResponse> MapToResponse(IEnumerable<CommentHistory> commentHistories, string language)
    {
        List<CommentHistoryResponse> response = commentHistories
            .Select(ch => new CommentHistoryResponse
            {
                CreationDate = ch.CreationDate,
                UserId = ch.UserId,
                ContextId = ch.ContextId,
                DisclosureRequirementId = ch.DisclosureRequirementId,
                Event = GetLocalizedReason(ch.Reason, language)
            })
            .ToList();

        return response;
    }

    private static string GetLocalizedReason(CommentHistoryReason reason, string language)
    {
        return reason switch
        {
            CommentHistoryReason.Created => language switch
            {
                "de" => "Kommentar erstellt",
                _ => "Comment created"
            },
            _ => language switch
            {
                "de" => "Unbekanntes Ereignis",
                _ => "Unknown event"
            }
        };
    }
}
