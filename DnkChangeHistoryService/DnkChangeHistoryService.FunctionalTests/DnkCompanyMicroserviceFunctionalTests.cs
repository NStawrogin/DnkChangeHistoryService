using DnkCompanyMicroservice.Domain.Company;
using DnkCompanyMicroservice.Domain;
using DnkCompanyMicroservice.Infrastructure.Rabbit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using DnkCompanyMicroService.FunctionalTests._Shared;
using NUnit.Framework;
using System.Text.Json;
using Assert = NUnit.Framework.Assert;
using CategoryAttribute = NUnit.Framework.CategoryAttribute;
using DnkCompanyMicroservice;
using System.Net;
using System.Text;
using DnkCompanyMicroservice.Application.Companies.Queries.Details;
using DnkCompanyMicroservice.Application.CompanyDeletionFlags.Create;
using Testcontainers.RabbitMq;

namespace DnkCompanyMicroService.FunctionalTests;

[Category(GlobalTestSettings.ExcludeFromLiveUnitTesting)]
[TestFixture]
public class DnkCompanyMicroserviceFunctionalTests
{
    private RabbitMqContainer? rabbitMqContainer;
    private WebApplicationFactory<Program>? factory;
    private readonly CompanyWebApplicationFactory companyWebApplicationFactory;
    private HttpClient httpClient;
    private const string BaseVersionRoute = "api/v1/";
    private readonly Guid invalidCompanyId = Guid.Parse("45cda9ae-8e70-475e-9489-f0eab23bdb4d");
    private readonly TestCompanyCacheStorage testCompanyCacheStorage;
    private static readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

    public DnkCompanyMicroserviceFunctionalTests(MsSqlTestProvider companyDatabaseProvider, TestCompanyCacheStorage testCompanyCacheStorage)
    {
        WebApplicationFactoryClientOptions clientOptions = new()
        {
            AllowAutoRedirect = false
        };

        this.companyWebApplicationFactory = new CompanyWebApplicationFactory(companyDatabaseProvider.GetConnectionString());
        this.httpClient = this.companyWebApplicationFactory.CreateClient(clientOptions);
        this.testCompanyCacheStorage = testCompanyCacheStorage;
    }

    private void InitializeFactory(Action<IServiceCollection> action)
    {
        this.factory = new WebApplicationFactory<Program>()
          .WithWebHostBuilder(builder =>
          {
              builder.ConfigureServices(services => action(services));
          });
        this.httpClient = this.factory.CreateClient();
    }

    private void SetDefaultContainerServices(IServiceCollection services)
    {
        RabbitConnectionFactoryProvider rabbitConnectionFactoryProviderInstance = new RabbitConnectionFactoryProviderMock(new Uri(this.rabbitMqContainer!.GetConnectionString()));
        services.AddSingleton(rabbitConnectionFactoryProviderInstance);
    }

    [OneTimeSetUp]
    public Task SetupAsync()
    {
        this.rabbitMqContainer = new RabbitMqBuilder()
            .WithImage("rabbitmq:4.0-management")
            .Build();

        return this.rabbitMqContainer.StartAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (this.rabbitMqContainer != null)
        {
            await this.rabbitMqContainer.DisposeAsync();
        }
    }

    [TearDown]
    public void TearDown()
    {
        this.factory?.Dispose();
    }

    [Test]
    public async Task RabbitListener_ShouldProcessCompanyDeletionMessage()
    {
        // Arrange
        this.InitializeFactory(this.SetDefaultContainerServices);
        const string queueName = "DefaultCompanyDeletion";

        ConnectionFactory connectionFactory = new()
        {
            Uri = new Uri(this.rabbitMqContainer!.GetConnectionString())
        };
        using IConnection connection = await connectionFactory.CreateConnectionAsync();
        IChannel channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queueName, false, false, false, null);

        CompanyDeletionMessage testMessage = new(DateTime.UtcNow, "Message");
        byte[] body = JsonSerializer.SerializeToUtf8Bytes(testMessage);

        AsyncEventingBasicConsumer consumer = new(channel);
        bool messageProcessed = false;

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            byte[] receivedBody = eventArgs.Body.ToArray();
            CompanyDeletionMessage? receivedMessage = JsonSerializer.Deserialize<CompanyDeletionMessage>(receivedBody);

            if (receivedMessage != null)
            {
                messageProcessed = true;
            }

            await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
        };

        // Act
        await channel.BasicPublishAsync("", queueName, body: body);
        await channel.BasicConsumeAsync(queueName, false, consumer);

        // Assert
        await Task.Delay(500);
        Assert.That(messageProcessed, Is.True);
    }

    [Test]
    public async Task RabbitMqProducer_ShouldSendCompanyCleanupMessage()
    {
        // Arrange
        this.InitializeFactory(this.SetDefaultContainerServices);
        const string queueName = "CompanyClaimsCleanupQueue";

        ConnectionFactory connectionFactory = new()
        {
            Uri = new Uri(this.rabbitMqContainer!.GetConnectionString())
        };
        using IConnection connection = await connectionFactory.CreateConnectionAsync();
        IChannel channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queueName, false, false, false, null);

        MessageBodyConverter messageBodyConverter = new MessageBodyConverter();
        DefaultCompanyClaimsCleanupProducer mockProducer = new DefaultCompanyClaimsCleanupProducer(new RabbitConnectionFactoryProviderMock(new Uri(this.rabbitMqContainer!.GetConnectionString())), messageBodyConverter);
        CompanyCleanupMessage testMessage = new(new List<Guid> { Guid.NewGuid() }, "Cleanup reason");
        await mockProducer.ProduceMessage(testMessage);

        AsyncEventingBasicConsumer consumer = new(channel);
        bool messageReceived = false;

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            byte[] receivedBody = eventArgs.Body.ToArray();
            CompanyCleanupMessage? receivedMessage = JsonSerializer.Deserialize<CompanyCleanupMessage>(receivedBody);

            if (receivedMessage != null && receivedMessage.Reason == testMessage.Reason)
            {
                messageReceived = true;
            }

            await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
        };

        // Act
        await channel.BasicConsumeAsync(queueName, false, consumer);

        // Assert
        await Task.Delay(500);
        Assert.That(messageReceived, Is.True);
    }

    [Test]
    public async Task CreateCompany_ShouldReturnOk_WhenValidCompany()
    {
        // Arrange        
        MultipartFormDataContent formContent = new()
        {
            { new StringContent("CompanyName"), "Name"},
            { new StringContent("079b6a60-737c-42c0-b344-2e59a5d02386"), "BusinessBranchId"},
            { new StringContent("c1540546-b95b-49ba-8bcf-9e95ee637e3c"), "CompanyHeadCountId"},
            { new StringContent("www.test.de"), "WebsiteUrl"},
            { new StringContent("43a3fb3b-7661-4bd6-a5c1-83e428bdda4f"), "CompanyTypeId"},
            { new StringContent("18163BA7-29E8-4796-B9BF-045EB078F8EF"), "CountryId"},
            { new StringContent("true"), "StockExchangeListed"},
            { new StringContent("2247"), "FullTimeEquivalents"},
            { new StringContent("1137"), "HeadCount"}
        };

        // Act
        HttpResponseMessage response = await this.httpClient.PostAsync($"{BaseVersionRoute}company", formContent);

        string responseString = await response.Content.ReadAsStringAsync();
        this.testCompanyCacheStorage.TestCompanyId = JsonSerializer.Deserialize<Guid>(responseString);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetCompany_ShouldHaveDeleteFlag_WhenCompanyIsFlagged()
    {
        // Arrange        
        Guid companyId = this.testCompanyCacheStorage.TestCompanyId!.Value;

        // Act
        HttpResponseMessage response = await this.httpClient.GetAsync($"{BaseVersionRoute}company?id={companyId}&language=de");

        string responseString = await response.Content.ReadAsStringAsync();
        CompanyDetailsResponse? companyDetailsResponse = JsonSerializer.Deserialize<CompanyDetailsResponse>(responseString, options);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(companyDetailsResponse, Is.Not.Null);
        Assert.That(companyDetailsResponse!.DeletionDate, Is.Not.Null);
    }

    [Test]
    public async Task AddDeleteFlag_ShouldReturnBadRequest_WhenCompanyNotInDb()
    {
        // Arrange
        Guid userId = Guid.Parse("c9f8e9da-89e7-464d-8224-e2f3f6c55a60");

        // Act
        FlagCompanyForDeletionCommand createDeleteCompanyFlagCommand = new()
        {
            CompanyId = this.invalidCompanyId,
            CreatedByUserId = userId
        };
        string payload = JsonSerializer.Serialize(createDeleteCompanyFlagCommand);
        StringContent content = new(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await this.httpClient.PostAsync($"{BaseVersionRoute}company/flag/delete", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task AddDeleteFlag_ShouldAddFlag_WhenValidCommand()
    {
        // Arrange        
        Guid userId = Guid.Parse("c9f8e9da-89e7-464d-8224-e2f3f6c55a60");

        // Act
        FlagCompanyForDeletionCommand createDeleteCompanyFlagCommand = new()
        {
            CompanyId = this.testCompanyCacheStorage.TestCompanyId!.Value,
            CreatedByUserId = userId
        };
        string payload = JsonSerializer.Serialize(createDeleteCompanyFlagCommand);
        StringContent content = new(payload, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await this.httpClient.PostAsync($"{BaseVersionRoute}company/flag/delete", content);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task RemoveCompanyDeletionFlag_ShouldReturnStatus200_WhenCompanyIsFlagged()
    {
        // Arrange        
        Guid companyId = this.testCompanyCacheStorage.TestCompanyId!.Value;

        // Act
        HttpResponseMessage response = await this.httpClient.DeleteAsync($"{BaseVersionRoute}company/flag/delete?companyId={companyId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetCompany_ShouldNotLongerHaveDeleteFlag_WhenCompanyFlagWasDeleted()
    {
        // Arrange        
        Guid companyId = this.testCompanyCacheStorage.TestCompanyId!.Value;

        // Act
        HttpResponseMessage response = await this.httpClient.GetAsync($"{BaseVersionRoute}company?id={companyId}&language=de");
        string responseString = await response.Content.ReadAsStringAsync();
        CompanyDetailsResponse? companyDetailsResponse = JsonSerializer.Deserialize<CompanyDetailsResponse>(responseString, options);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(companyDetailsResponse, Is.Not.Null);
        Assert.That(companyDetailsResponse!.DeletionDate, Is.Null);
    }
}
