using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using NUnit.Framework;
using Testcontainers.MsSql;

namespace DnkCompanyMicroservice.ContainerTests;

[SetUpFixture]
public class ContainerSetUpFixture
{
    public static ushort MappedPort { get; private set; }

    private IFutureDockerImage companyMicroServiceImage = null!;
    private INetwork network = null!;
    private MsSqlContainer msSqlContainer = null!;
    private IContainer companyMicroServiceContainer = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        string storage = "storage";

        string connectionString = $"server={storage};user id={MsSqlBuilder.DefaultUsername};password={MsSqlBuilder.DefaultPassword};database={MsSqlBuilder.DefaultDatabase};TrustServerCertificate=True";

        this.companyMicroServiceImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
            .WithDockerfile("DnkCompanyMicroservice/Dockerfile")
            .WithName("dnk-company-microservice")
            .WithDeleteIfExists(false)
            .Build();

        this.network = new NetworkBuilder().Build();

        this.msSqlContainer = new MsSqlBuilder()
            .WithNetwork(this.network)
            .WithNetworkAliases(storage)
            .Build();

        this.companyMicroServiceContainer = new ContainerBuilder()
            .WithImage(this.companyMicroServiceImage)
            .WithNetwork(this.network)
            .WithPortBinding(8080, true)
            .WithEnvironment("ConnectionStrings__DNkMigrationDbConnection", connectionString)
            .WithEnvironment("ConnectionStrings__DnkMicroServiceDbConnection", connectionString)
            .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080))
            .Build();

        await this.companyMicroServiceImage
            .CreateAsync()
            .ConfigureAwait(false);

        await this.network
            .CreateAsync()
            .ConfigureAwait(false);

        await this.msSqlContainer
            .StartAsync()
            .ConfigureAwait(false);

        await this.companyMicroServiceContainer
            .StartAsync()
            .ConfigureAwait(false);

        MappedPort = this.companyMicroServiceContainer.GetMappedPublicPort(8080);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await this.companyMicroServiceImage.DisposeAsync().ConfigureAwait(false);
        await this.network.DisposeAsync().ConfigureAwait(false);
        await this.msSqlContainer.DisposeAsync().ConfigureAwait(false);
        await this.companyMicroServiceContainer.DisposeAsync().ConfigureAwait(false);
    }
}

