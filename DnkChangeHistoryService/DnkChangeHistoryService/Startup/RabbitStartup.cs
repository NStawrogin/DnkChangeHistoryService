using DnkChangeHistoryService.Application.Rabbit;
using DnkChangeHistoryService.Domain.ChangeHistory;
using DnkChangeHistoryService.Domain.Rabbit;
using DnkChangeHistoryService.Infrastructure.Rabbit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace DnkChangeHistoryService.Startup;

[ExcludeFromCodeCoverage]
internal static class RabbitStartup
{
    public static void RegisterRabbitServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<RabbitConnectionFactoryProvider>();
        builder.Services.Configure<RabbitConfig>(builder.Configuration.GetSection("RabbitMQ"));
        builder.Services.TryAddSingleton<RabbitConfig>(x => x.GetService<IOptions<RabbitConfig>>()!.Value);
        builder.Services.AddSingleton<RabbitListener>();
        builder.Services.AddSingleton<IMessageBodyConverter, MessageBodyConverter>();
        builder.Services.AddHostedService<HostedRabbitMqService>();
    }
}