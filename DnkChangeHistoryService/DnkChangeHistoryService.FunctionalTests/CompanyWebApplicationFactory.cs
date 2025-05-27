using DnkCompanyMicroservice;
using DnkCompanyMicroservice.Infrastructure.Context;
using DnkCompanyMicroService.FunctionalTests._Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace DnkCompanyMicroService.FunctionalTests;
public class CompanyWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            RegisterDbContexts(services, connectionString);
        });
    }

    private static void RegisterDbContexts(IServiceCollection services, string connectionString)
    {
        ServiceDescriptorHelper.RemoveServiceDescriptor<DbContextOptions<CompanyWorkContext>>(services);
        ServiceDescriptorHelper.RemoveServiceDescriptor<DbContextOptions<CompanyMigrationContext>>(services);
        ServiceDescriptorHelper.RemoveServiceDescriptor<DbConnection>(services);

        //Ersetzung auch im work context mit db owner connection string, da keine Sql-User-Rechte geprüft werden.
        services.AddDbContextFactory<CompanyWorkContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddDbContextFactory<CompanyMigrationContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    }
}
