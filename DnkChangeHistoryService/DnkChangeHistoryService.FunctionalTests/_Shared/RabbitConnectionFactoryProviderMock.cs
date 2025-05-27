using DnkCompanyMicroservice.Domain.Rabbit;
using DnkCompanyMicroservice.Infrastructure.Rabbit;
using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;

namespace DnkCompanyMicroService.FunctionalTests._Shared;

[ExcludeFromCodeCoverage]
/// <summary>
/// Needed due to the fact that testcontainer of RabbitMq is connected by uri
/// </summary>
/// <param name="uri">RabbitMq testcontainer uri</param>
public class RabbitConnectionFactoryProviderMock(Uri uri) : RabbitConnectionFactoryProvider(new RabbitConfig())
{
    private readonly Uri uri = uri;

    public override IConnectionFactory CreateConnectionFactory()
    {
        return new ConnectionFactory() { Uri = this.uri };
    }
}
