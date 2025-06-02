using DnkChangeHistoryService.Infrastructure.Context;
using DnkChangeHistoryService.Startup;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService;

[ExcludeFromCodeCoverage]
public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHttpLogging(o => { });

        builder.Services.AddAuthorization();

        string? connectionString = builder.Configuration.GetConnectionString("DnkMicroServiceDbConnection");
        builder.Services.AddDbContextFactory<ChangeHistoryWorkContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        string? connectionStringMigration = builder.Configuration.GetConnectionString("DNkMigrationDbConnection");
        builder.Services.AddDbContextFactory<ChangeHistoryMigrationContext>(options =>
        {
            options.UseSqlServer(connectionStringMigration);
        });

        RegisterServices(builder);

        WebApplication app = builder.Build();

        app.UseHttpLogging();

        await DatabaseStartup.ApplyMigrations(app);

        HealthCheckEndpoint.ConfigureHealthChecks(app);

        app.UseAuthorization();
        app.UseHttpsRedirection();

        app.MapEndpoints();

        if (app.Environment.IsDevelopment())
        {
            OpenApiStartup.ConfigureSwagger(app);
        }

        await app.RunAsync();
    }

    private static void RegisterServices(WebApplicationBuilder builder)
    {
        VersioningStartup.RegisterVersioningServices(builder);

        HealthCheckEndpoint.RegisterHealthCheckServices(builder);

        OpenApiStartup.RegisterSwaggerServices(builder);

        RabbitStartup.RegisterRabbitServices(builder);

        CommentHistoryServiceStartup.RegisterCommentHistoryService(builder);

        builder.Services.AddChangeHistoryServiceApplication();
    }
}