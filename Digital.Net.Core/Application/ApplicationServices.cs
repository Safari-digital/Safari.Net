using Microsoft.Extensions.DependencyInjection;

namespace Digital.Net.Core.Application;

public static class ApplicationServices
{
    public static bool IsInjected(this IServiceCollection services, Type? serviceType) =>
        services.Any(service => service.ServiceType == serviceType);
}