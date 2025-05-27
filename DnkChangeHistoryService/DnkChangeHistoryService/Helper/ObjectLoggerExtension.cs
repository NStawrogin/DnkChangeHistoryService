using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace DnkChangeHistoryService.Helper;

[ExcludeFromCodeCoverage]
public static class ObjectLoggerExtension
{
    public static string SerializeObject<T>(this T obj) where T : class
    {
        return JsonSerializer.Serialize(obj);
    }
}