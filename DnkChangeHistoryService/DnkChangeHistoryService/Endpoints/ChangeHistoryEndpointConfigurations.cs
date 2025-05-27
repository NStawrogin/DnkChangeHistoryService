using DnkCompanyMicroservice.Application.Companies.Commands.Create;
using DnkCompanyMicroservice.Application.Companies.Commands.Update;
using DnkCompanyMicroservice.Application.Companies.Queries.Details;
using DnkCompanyMicroservice.Application.Companies.Queries.LegacyInformation;
using DnkCompanyMicroservice.Application.Companies.Queries.Names;
using DnkCompanyMicroservice.Application.Companies.Queries.Overview;
using DnkCompanyMicroservice.Application.Companies.Queries.Update;
using DnkCompanyMicroservice.Application.CompanyDeletion;
using DnkCompanyMicroservice.Application.CompanyDeletionFlags;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Endpoints;

[ExcludeFromCodeCoverage]
internal static class ChangeHistoryEndpointConfigurations
{
    private const string CompanyApiTag = "ChangeHistoryApi";

    public static void ConfigureChangeHistoryEndpoints(this IEndpointRouteBuilder app)
    {

    }
}