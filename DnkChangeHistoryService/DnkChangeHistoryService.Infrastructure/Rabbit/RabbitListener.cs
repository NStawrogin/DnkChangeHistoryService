using DnkChangeHistoryService.Domain.ChangeHistory;
using DnkChangeHistoryService.Domain.ChangeHistory.CommentHistory;
using DnkChangeHistoryService.Infrastructure.CommentHistories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace DnkChangeHistoryService.Infrastructure.Rabbit;

[ExcludeFromCodeCoverage(Justification = "Can not unit test RabbitMQ connection")]
public class RabbitListener(ILogger<RabbitListener> logger,
                            IMessageBodyConverter consumerConverter,
                            RabbitConnectionFactoryProvider rabbitConnectionFactory,
                            IServiceProvider serviceProvider) : IRabbitListener
{
    private readonly RabbitConnectionFactoryProvider rabbitConnectionFactory = rabbitConnectionFactory;
    private readonly ILogger<RabbitListener> logger = logger;
    private readonly IMessageBodyConverter consumerConverter = consumerConverter;
    private readonly IServiceProvider serviceProvider = serviceProvider;
    private IChannel? channel;
    private const string DefaultCommentHistoryQueue = "DefaultCommentHistory";

    public async Task RegisterDefaultCommentHistoryQueue()
    {
        this.logger.LogInformation("Registering for receive CommentHistory messages.");
        this.channel = await this.rabbitConnectionFactory.CreateChannelAsync();
        await this.channel.QueueDeclareAsync(queue: DefaultCommentHistoryQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);

        AsyncEventingBasicConsumer consumer = new(channel: this.channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            ChangeHistoryState result = await this.ProcessCommentHistoryMessageAsync(ea, CancellationToken.None);
            switch (result)
            {
                case ChangeHistoryState.Failed:
                    await this.channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    break;
                case ChangeHistoryState.Succesfull:
                    await this.channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                    break;
                default:
                    this.logger.LogError("Unknown CommentHistory state");
                    break;
            }
        };
        await this.channel.BasicConsumeAsync(queue: DefaultCommentHistoryQueue, autoAck: false, consumer: consumer);
        this.logger.LogInformation("Waiting to receive CommentHistory messages.");
    }

    private async Task<ChangeHistoryState> ProcessCommentHistoryMessageAsync(BasicDeliverEventArgs ea, CancellationToken cancellationToken = default)
    {
        try
        {
            using IServiceScope scope = this.serviceProvider.CreateScope();
            ICommentHistoryService commentHistoryService = scope.ServiceProvider.GetRequiredService<ICommentHistoryService>();

            CommentHistoryMessage? commentHistoryMessage = this.consumerConverter.Convert<CommentHistoryMessage>(ea.Body.ToArray());

            if (commentHistoryMessage != null)
            {
                this.logger.LogInformation("Received CommentHistoryMessage with Reason: {Reason} at {Timestamp}", commentHistoryMessage.Reason, commentHistoryMessage.Timestamp);

                await commentHistoryService.HandleCommentHistoryAsync(commentHistoryMessage, cancellationToken);

                return ChangeHistoryState.Succesfull;
            }
            else
            {
                this.logger.LogWarning("Received empty or invalid CommentHistoryMessage.");
                return ChangeHistoryState.Failed;
            }
        }
        catch (JsonException ex)
        {
            this.logger.LogError("Invalid JSON format: {Error}", ex);
            return ChangeHistoryState.Failed;
        }
        catch (Exception ex)
        {
            this.logger.LogError("Error processing CommentHistoryMessage: {Error}", ex);
            return ChangeHistoryState.Failed;
        }
    }

    public async Task CloseChannelConnectionAsync()
    {
        await this.rabbitConnectionFactory.CloseChannelConnectionAsync();
    }
}
