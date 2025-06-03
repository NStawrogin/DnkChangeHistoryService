using DnkChangeHistoryService.Domain.Health;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace DnkChangeHistoryService.Startup;

internal static class HealthCheckEndpoint
{
    /// <summary>
    /// Konfiguration der Healthchecks für Kubernetes
    /// https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health
    /// </summary>
    /// <param name="app"></param>
    public static void ConfigureHealthChecks(WebApplication app)
    {
        app.MapHealthChecks("/healthcheck", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    }

    /// <summary>
    /// Fügt Healthcheck service für Kubernetes hinzu
    /// https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/monitor-app-health
    /// </summary>
    /// <param name="builder"></param>
    public static void RegisterHealthCheckServices(WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks().AddCheck<HealthCheck>("Change History health");
    }
}
