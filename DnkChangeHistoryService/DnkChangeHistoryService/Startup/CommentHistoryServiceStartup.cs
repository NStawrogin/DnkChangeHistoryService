using DnkChangeHistoryService.Infrastructure.CommentHistories;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Startup;

[ExcludeFromCodeCoverage]
internal static class CommentHistoryServiceStartup
{
    public static void RegisterCommentHistoryService(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICommentHistoryService, CommentHistoryService>();
    }
}
