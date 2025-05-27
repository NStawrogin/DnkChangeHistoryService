//using DnkCompanyMicroservice.Domain.Company;
//using DnkCompanyMicroservice.Infrastructure.Companies;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System.Diagnostics.CodeAnalysis;
//using System.Text.Json;

//namespace DnkCompanyMicroservice.Infrastructure.Rabbit;

//[ExcludeFromCodeCoverage(Justification = "Can not unit test RabbitMQ connection")]
//public class RabbitListener(ILogger<RabbitListener> logger,
//                            IMessageBodyConverter consumerConverter,
//                            RabbitConnectionFactoryProvider rabbitConnectionFactory,
//                            IServiceProvider serviceProvider) : IRabbitListener
//{
//    private readonly RabbitConnectionFactoryProvider rabbitConnectionFactory = rabbitConnectionFactory;
//    private readonly ILogger<RabbitListener> logger = logger;
//    private readonly IMessageBodyConverter consumerConverter = consumerConverter;
//    private readonly IServiceProvider serviceProvider = serviceProvider;
//    private IChannel? channel;
//    private const string DefaultCompanyDeletionQueue = "DefaultCompanyDeletion";

//    public async Task RegisterDefaultDeleteCompanyQueue()
//    {
//        this.logger.LogInformation("Registering for receive CompanyDeletion messages.");
//        this.channel = await this.rabbitConnectionFactory.CreateChannelAsync();
//        await this.channel.QueueDeclareAsync(queue: DefaultCompanyDeletionQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);

//        AsyncEventingBasicConsumer consumer = new(channel: this.channel);
//        consumer.ReceivedAsync += async (model, ea) =>
//        {
//            CompanyDeletionState result = await this.ProcessDeletionMessageAsync(ea, CancellationToken.None);
//            switch (result)
//            {
//                case CompanyDeletionState.Failed:
//                    await this.channel.BasicNackAsync(ea.DeliveryTag, false, true);
//                    break;
//                case CompanyDeletionState.Succesfull:
//                    await this.channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
//                    break;
//                default:
//                    this.logger.LogError("Unknown CompanyDeletion state");
//                    break;
//            }
//        };
//        await this.channel.BasicConsumeAsync(queue: DefaultCompanyDeletionQueue, autoAck: false, consumer: consumer);
//        this.logger.LogInformation("Waiting to receive CompanyDeletion messages.");
//    }

//    private async Task<CompanyDeletionState> ProcessDeletionMessageAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken = default)
//    {
//        try
//        {
//            using IServiceScope scope = this.serviceProvider.CreateScope();
//            IDeleteCompanyService deleteCompanyService = scope.ServiceProvider.GetRequiredService<IDeleteCompanyService>();

//            CompanyDeletionMessage? deletionMessage = this.consumerConverter.Convert<CompanyDeletionMessage>(ea.Body.ToArray());

//            if (deletionMessage != null)
//            {
//                this.logger.LogInformation("Received CompanyDeletionMessage at {Timestamp}", deletionMessage.DeletionTimestamp);

//                List<Guid> companiesToDelete = await deleteCompanyService.GetCompaniesToDelete(deletionMessage.DeletionTimestamp, cancellationToken);

//                if (companiesToDelete.Count > 0)
//                {
//                    this.logger.LogInformation("Companies to delete: {Companies}", companiesToDelete);

//                    this.logger.LogInformation("Deleting companies...");

//                    bool success = await deleteCompanyService.DeleteCompaniesAndFlags(companiesToDelete, cancellationToken);

//                    if (!success)
//                    {
//                        this.logger.LogError("Error deleting companies.");
//                        return CompanyDeletionState.Failed;
//                    }

//                    this.logger.LogInformation("Companies deleted.");

//                    this.logger.LogInformation("Sending CompanyCleanupMessages...");

//                    CompanyCleanupMessage cleanupMessage = new(companiesToDelete, deletionMessage.Reason);

//                    await this.SendCleanupMessagesAsync(companiesToDelete, deletionMessage.Reason, scope);

//                    this.logger.LogInformation("CompanyCleanupMessages sent.");
//                }

//                return CompanyDeletionState.Succesfull;
//            }
//            else
//            {
//                this.logger.LogWarning("Received empty or invalid CompanyDeletionMessage.");
//                return CompanyDeletionState.Failed;
//            }
//        }
//        catch (JsonException ex)
//        {
//            this.logger.LogError("Invalid JSON format: {Error}", ex);
//            return CompanyDeletionState.Failed;
//        }
//        catch (Exception ex)
//        {
//            this.logger.LogError("Error processing CompanyDeletionMessage: {Error}", ex);
//            return CompanyDeletionState.Failed;
//        }
//    }

//    private async Task SendCleanupMessagesAsync(List<Guid> companiesToDelete, string reason, IServiceScope scope)
//    {
//        IRabbitMqProducer<CompanyCleanupMessage> claimsCleanupProducer = scope.ServiceProvider.GetRequiredKeyedService<IRabbitMqProducer<CompanyCleanupMessage>>("ClaimsCleanup");
//        IRabbitMqProducer<CompanyCleanupMessage> reportCleanupProducer = scope.ServiceProvider.GetRequiredKeyedService<IRabbitMqProducer<CompanyCleanupMessage>>("ReportCleanup");

//        CompanyCleanupMessage cleanupMessage = new(companiesToDelete, reason);

//        await claimsCleanupProducer.ProduceMessage(cleanupMessage);
//        await reportCleanupProducer.ProduceMessage(cleanupMessage);
//    }

//    public async Task CloseChannelConnectionAsync()
//    {
//        await this.rabbitConnectionFactory.CloseChannelConnectionAsync();
//    }
//}
