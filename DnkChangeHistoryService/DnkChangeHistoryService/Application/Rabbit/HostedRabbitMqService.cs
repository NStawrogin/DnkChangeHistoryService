using DnkChangeHistoryService.Infrastructure.Rabbit;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Application.Rabbit;

[ExcludeFromCodeCoverage]
public sealed class HostedRabbitMqService(
    ILogger<HostedRabbitMqService> logger, RabbitListener rabbitListener) : IHostedService, IHostedLifecycleService
{
    private readonly ILogger logger = logger;
    private readonly RabbitListener listener = rabbitListener;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("HostedRabbitMqService start");
        return Task.CompletedTask;
    }

    public Task StartedAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("HostedRabbitMqService started");
        return this.listener.RegisterDefaultCommentHistoryQueue();
    }

    public Task StartingAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("HostedRabbitMqService starting");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("HostedRabbitMqService stop");
        return Task.CompletedTask;
    }

    public Task StoppedAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("HostedRabbitMqService stopped");
        return Task.CompletedTask;
    }

    public Task StoppingAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("HostedRabbitMqService stopping");
        return this.listener.CloseChannelConnectionAsync();
    }
}
