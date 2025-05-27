using Testcontainers.MsSql;

namespace DnkCompanyMicroService.FunctionalTests._Shared;
public class MsSqlTestProvider : IAsyncLifetime
{
    private readonly MsSqlContainer msSqlContainer;

    public MsSqlTestProvider()
    {
        this.msSqlContainer = new MsSqlBuilder()
          .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
          .Build();
    }

    public Task InitializeAsync()
    {
        return this.msSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return this.msSqlContainer.DisposeAsync().AsTask();
    }

    public string GetConnectionString()
    {
        return this.msSqlContainer.GetConnectionString();
    }
}