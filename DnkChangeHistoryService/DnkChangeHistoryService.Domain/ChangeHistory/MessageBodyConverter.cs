using System.Text;
using System.Text.Json;

namespace DnkChangeHistoryService.Domain.ChangeHistory;

public class MessageBodyConverter : IMessageBodyConverter
{
    public T? Convert<T>(byte[] body)
    {
        string message = Encoding.UTF8.GetString(body);
        return JsonSerializer.Deserialize<T>(message);
    }

    public byte[] ConvertToBytes<T>(T message)
    {
        string serialzed = JsonSerializer.Serialize(message);
        byte[] body = Encoding.UTF8.GetBytes(serialzed);
        return body;
    }
}

