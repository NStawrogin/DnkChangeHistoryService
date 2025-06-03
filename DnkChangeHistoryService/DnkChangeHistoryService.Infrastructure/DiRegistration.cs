using DnkChangeHistoryService.Infrastructure.Repositories.CommentHistories.Commands;
using DnkChangeHistoryService.Infrastructure.Repositories.CommentHistories.Queries;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Infrastructure;

/// <summary>
/// Dependency Injection registration class
/// </summary>
[ExcludeFromCodeCoverage]
public static class DiRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ICommentHistoryQueryRepository, CommentHistoryQueryRepository>();
        services.AddScoped<ICommentHistoryCommandRepository, CommentHistoryCommandRepository>();
    }
}