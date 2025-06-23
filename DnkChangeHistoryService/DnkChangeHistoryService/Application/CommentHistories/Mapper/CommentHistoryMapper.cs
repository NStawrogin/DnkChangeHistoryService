using DnkChangeHistoryService.Application.CommentHistories.Queries.History;
using DnkChangeHistoryService.Domain.ChangeHistory.CommentHistory;
using DnkChangeHistoryService.Domain.ChangeHistoryManagement;

namespace DnkChangeHistoryService.Application.CommentHistories.Mapper;

public class CommentHistoryMapper : ICommentHistoryMapper
{
    private const string GermanLanguageCode = "de";

    public List<CommentHistoryResponse> MapToResponse(IEnumerable<CommentHistory> commentHistories, string language)
    {
        List<CommentHistoryResponse> response = commentHistories
            .Select(ch => new CommentHistoryResponse
            {
                CommentId = ch.CommentId,
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
                GermanLanguageCode => "Kommentar erstellt",
                _ => "Comment created"
            },
            CommentHistoryReason.Closed => language switch
            {
                GermanLanguageCode => "Kommentar abgeschlossen",
                _ => "Comment closed"
            },
            CommentHistoryReason.SubTaskClosed => language switch
            {
                GermanLanguageCode => "Teilaufgabe abgeschlossen",
                _ => "Subtask closed"
            },
            CommentHistoryReason.SubTaskReopened => language switch
            {
                GermanLanguageCode => "Teilaufgabe wiedereröffnet",
                _ => "Subtask reopened"
            },
            CommentHistoryReason.Replied => language switch
            {
                GermanLanguageCode => "Kommentar beantwortet",
                _ => "Replied to comment"
            },
            _ => language switch
            {
                GermanLanguageCode => "Unbekanntes Ereignis",
                _ => "Unknown event"
            }
        };
    }
}
