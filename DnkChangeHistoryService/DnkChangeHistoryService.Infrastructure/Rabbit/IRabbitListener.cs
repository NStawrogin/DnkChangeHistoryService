namespace DnkChangeHistoryService.Infrastructure.Rabbit;

public interface IRabbitListener
{
    Task RegisterDefaultCommentHistoryQueue();
}
