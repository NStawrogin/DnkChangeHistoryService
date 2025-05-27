using Asp.Versioning.ApiExplorer;
using DnkChangeHistoryService.OpenApi;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Startup;

[ExcludeFromCodeCoverage]
public static class OpenApiStartup
{
    public static void ConfigureSwagger(WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            IReadOnlyList<ApiVersionDescription> descriptions = app.DescribeApiVersions();

            foreach (ApiVersionDescription description in descriptions)
            {
                string url = $"/swagger/{description.GroupName}/swagger.json";
                string name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
            }
            options.RoutePrefix = "swagger";
        });
    }

    /// <summary>
    /// Für OpenAPI Dokumentation
    /// </summary>
    /// <param name="builder"></param>
    public static void RegisterSwaggerServices(WebApplicationBuilder builder)
    {
        builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }
}