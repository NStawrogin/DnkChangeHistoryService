using DnkChangeHistoryService.Application.CommentHistories.Queries.History;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Endpoints;

[ExcludeFromCodeCoverage]
internal static class ChangeHistoryEndpointConfigurations
{
    private const string ChangeHistoryApiTag = "ChangeHistoryApi";

    public static void ConfigureChangeHistoryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/history/commenthistory",
            [OpenApiOperation("Get comment history", "Get comment history data by reportId, contextId and language")]
            async (
                    [FromServices] ICommentHistoryQueryHandler commentHistoryQueryHandler,
                    [FromQuery] Guid reportId,
                    [FromQuery] string contextId,
                    [FromQuery][Length(2, 2)] string language,
                    CancellationToken cancellationToken = default) =>
            {
                List<CommentHistoryResponse> response = await commentHistoryQueryHandler.Handle(new CommentHistoryQuery { ReportId = reportId, ContextId = contextId, Language = language }, cancellationToken);
                return Results.Ok(response);
            })
            .WithName("GetCommentHistory")
            .WithTags(ChangeHistoryApiTag)
            .Produces<List<CommentHistoryResponse>>();
    }
}