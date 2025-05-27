using DnkChangeHistoryService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Startup;

[ExcludeFromCodeCoverage]
internal static class DatabaseStartup
{
    public static async Task ApplyMigrations(WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        ChangeHistoryMigrationContext dbContext = scope.ServiceProvider.GetRequiredService<ChangeHistoryMigrationContext>();
        if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
        {
            await dbContext.Database.MigrateAsync();
        }
    }
}
