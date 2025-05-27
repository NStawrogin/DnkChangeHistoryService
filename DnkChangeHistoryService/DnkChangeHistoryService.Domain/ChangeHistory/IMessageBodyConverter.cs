namespace DnkChangeHistoryService.Domain.ChangeHistory;

public interface IMessageBodyConverter
{
    T? Convert<T>(byte[] body);
    byte[] ConvertToBytes<T>(T message);
}
