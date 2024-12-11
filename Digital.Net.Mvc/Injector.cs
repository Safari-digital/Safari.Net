using Digital.Net.Core.Application;
using Digital.Net.Mvc.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Digital.Net.Mvc;

public static class DigitalMvcInjector
{
    public static IServiceCollection AddDigitalMvc(this IServiceCollection services)
    {
        services.AddScoped<IHttpContextService, HttpContextService>();

        if (!services.IsInjected(typeof(IHttpContextAccessor)))
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        return services;
    }
}