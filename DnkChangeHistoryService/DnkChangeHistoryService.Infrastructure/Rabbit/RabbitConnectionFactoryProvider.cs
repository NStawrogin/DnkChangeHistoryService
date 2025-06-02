using DnkChangeHistoryService.Domain.Rabbit;
using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Infrastructure.Rabbit;

[ExcludeFromCodeCoverage(Justification = "Can not unit test RabbitMq connection")]
public class RabbitConnectionFactoryProvider(RabbitConfig rabbitConfig)
{
    private readonly RabbitConfig rabbitConfig = rabbitConfig;

    private IConnectionFactory? factory;
    private IConnection? connection;
    private IChannel? channel;

    public virtual IConnectionFactory CreateConnectionFactory()
    {
        ConnectionFactory connectionFactory = new() { HostName = rabbitConfig.HostName! };
        if (!string.IsNullOrEmpty(rabbitConfig.UserName))
        {
            connectionFactory.UserName = rabbitConfig.UserName;
        }
        if (!string.IsNullOrEmpty(rabbitConfig.Password))
        {
            connectionFactory.Password = rabbitConfig.Password;
        }

        return connectionFactory;
    }

    public virtual async Task<IConnection> CreateConnectionAsync()
    {
        this.factory ??= this.CreateConnectionFactory();
        this.connection ??= await this.factory.CreateConnectionAsync();
        return this.connection;
    }

    public virtual async Task<IChannel> CreateChannelAsync()
    {
        await this.CreateConnectionAsync();
        this.channel ??= await this.connection!.CreateChannelAsync();
        return this.channel;
    }

    public virtual async Task CloseChannelConnectionAsync()
    {
        if (this.channel != null)
        {
            await this.channel.CloseAsync();
            this.channel.Dispose();
            this.channel = null;
        }
        if (this.connection != null)
        {
            await this.connection.CloseAsync();
            this.connection.Dispose();
            this.connection = null;
        }
    }
}
