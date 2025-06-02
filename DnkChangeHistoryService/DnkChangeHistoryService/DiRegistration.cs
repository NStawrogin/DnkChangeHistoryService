using DnkChangeHistoryService.Application.CommentHistories.Mapper;
using DnkChangeHistoryService.Application.CommentHistories.Queries.History;
using DnkChangeHistoryService.Infrastructure;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService;

/// <summary>
/// A class to register services for DI
/// </summary>
[ExcludeFromCodeCoverage]
internal static class DiRegistration
{
    public static void AddChangeHistoryServiceApplication(this IServiceCollection services)
    {
        services.AddLocalServices();

        services.AddInfrastructureServices();
    }

    private static void AddLocalServices(this IServiceCollection services)
    {
        services.RegisterCommentHistoryServices();
    }

    private static void RegisterCommentHistoryServices(this IServiceCollection services)
    {
        services.AddScoped<ICommentHistoryQueryHandler, CommentHistoryQueryHandler>();
        services.AddScoped<ICommentHistoryMapper, CommentHistoryMapper>();
    }
}