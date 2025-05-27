namespace DnkChangeHistoryService.Domain.Helpers;

public interface IEntity
{
    public Guid Id { get; set; }

    public DateTime CreationDate { get; set; }
}