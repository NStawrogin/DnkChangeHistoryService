using Asp.Versioning;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Startup;

[ExcludeFromCodeCoverage]
public class VersioningStartup
{
    /// <summary>
    /// Für OpenAPI Dokumentation
    /// </summary>
    /// <param name="builder"></param>
    public static void RegisterVersioningServices(WebApplicationBuilder builder)
    {
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"));
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });
    }
}
