using Digital.Net.Authentication.Models;
using Digital.Net.Authentication.Options;
using Digital.Net.Authentication.Services;
using Digital.Net.Core.Application;
using Digital.Net.Entities.Repositories;
using Digital.Net.Mvc;
using Digital.Net.Mvc.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Digital.Net.Authentication;

public static class Injector
{
    /// <summary>
    ///     Add DigitalApiKeyAuthorization to the service collection. Use the
    ///     <see cref="DigitalApiKeyAuthorizationOptions" /> to configure the options.
    /// </summary>
    /// <remarks>
    ///     This service requires the following dependencies:
    ///     <ul>
    ///         <li>Digital.Net.Database</li>
    ///         <li>Digital.Net.Entities</li>
    ///         <li>Digital.Net.Mvc</li>
    ///     </ul>
    /// </remarks>
    /// <param name="services"> The service collection to add the service to. </param>
    /// <param name="buildOptions"> The action to build the options. </param>
    /// <typeparam name="TModel"> The EFCore model to be used for the API Key. </typeparam>
    /// <returns></returns>
    public static IServiceCollection AddDigitalApiKeyAuthorization<TModel>(
        this IServiceCollection services,
        Action<DigitalApiKeyAuthorizationOptions>? buildOptions = null
    )
        where TModel : ApiKeyEntity
    {
        services.ConfigureBuildOptions(buildOptions);
        services.AddScoped<IApiKeyService, ApiKeyService<TModel>>();

        if (!services.IsInjected(typeof(IHttpContextService)))
            services.AddDigitalMvc();
        if (!services.IsInjected(typeof(IRepository<TModel>)))
            services.AddScoped<IRepository<TModel>, Repository<TModel>>();

        return services;
    }

    private static void ConfigureBuildOptions(
        this IServiceCollection services,
        Action<DigitalApiKeyAuthorizationOptions>? buildOptions = null
    )
    {
        if (buildOptions is null)
            services.Configure<DigitalApiKeyAuthorizationOptions>(_ => { });
        else
            services.Configure(buildOptions);
    }
}