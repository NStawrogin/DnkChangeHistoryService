using Asp.Versioning;
using Asp.Versioning.Builder;
using DnkChangeHistoryService.Endpoints;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Startup;

[ExcludeFromCodeCoverage(Justification = "Unittesting too complicated")]
internal static class EndpointStartup
{
    public const string GatewayRoutePrefix = "api";

    public static void MapEndpoints(this IEndpointRouteBuilder routeBuilder)
    {
        ApiVersionSet apiVersionSet = routeBuilder.NewApiVersionSet()
                                        .HasApiVersion(new ApiVersion(1))
                                        .ReportApiVersions()
                                        .Build();

        RouteGroupBuilder groupBuilder = routeBuilder.MapGroup(GatewayRoutePrefix + "/v{version:apiVersion}")
            .WithApiVersionSet(apiVersionSet);

        groupBuilder.ConfigureChangeHistoryEndpoints();
    }