using DnkChangeHistoryService.Infrastructure.Context;
using DnkChangeHistoryService.FunctionalTests._Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace DnkChangeHistoryService.FunctionalTests;
public class ChangeHistoryWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
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
        ServiceDescriptorHelper.RemoveServiceDescriptor<DbContextOptions<ChangeHistoryWorkContext>>(services);
        ServiceDescriptorHelper.RemoveServiceDescriptor<DbContextOptions<ChangeHistoryMigrationContext>>(services);
        ServiceDescriptorHelper.RemoveServiceDescriptor<DbConnection>(services);

        //Ersetzung auch im work context mit db owner connection string, da keine Sql-User-Rechte geprüft werden.
        services.AddDbContextFactory<ChangeHistoryWorkContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddDbContextFactory<ChangeHistoryMigrationContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
    }
}
