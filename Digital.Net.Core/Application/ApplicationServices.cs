using Microsoft.Extensions.DependencyInjection;

namespace Digital.Net.Core.Application;

public static class ApplicationServices
{
    public static bool IsInjected(this IServiceCollection services, Type? serviceType) =>
        services.Any(service => service.ServiceType == serviceType);

    public static void ConfigureOptions<T>(
        this IServiceCollection services,
        Action<T>? buildOptions = null
    )
        where T : class, new()
    {
        if (buildOptions is null)
            services.Configure<T>(_ => { });
        else
            services.Configure(buildOptions);
    }
}