using DnkChangeHistoryService.Domain.Helpers;

namespace DnkChangeHistoryService.Domain.ChangeHistoryManagement;

public class CommentHistory : IEntity
{
    public Guid Id { get; set; }

    public DateTime CreationDate { get; set; }

    public string LanguageTwoLetterCode { get; set; } = string.Empty;
}