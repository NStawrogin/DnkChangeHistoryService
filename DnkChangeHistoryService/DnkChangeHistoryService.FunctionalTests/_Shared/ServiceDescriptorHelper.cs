using Microsoft.Extensions.DependencyInjection;

namespace DnkCompanyMicroService.FunctionalTests._Shared;
internal static class ServiceDescriptorHelper
{
    public static void RemoveServiceDescriptor<T>(IServiceCollection services)
    {
        ServiceDescriptor? serviceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        services.Remove(serviceDescriptor!);
    }
}