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

    public static string SanitizeLogValue(this string logValue)
    {
        return logValue.Replace(Environment.NewLine, "")
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace("\t", "");
    }
}