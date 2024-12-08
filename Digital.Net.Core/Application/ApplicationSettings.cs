using Digital.Net.Core.Environment;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Digital.Net.Core.Application;

public static class ApplicationSettings
{
    /// <summary>
    ///     Add application settings to the configuration builder from the provided project path.
    /// </summary>
    /// <param name="builder">The configuration builder.</param>
    /// <param name="projectName">The name of the project.</param>
    public static IConfigurationBuilder AddProjectSettings(this IConfigurationBuilder builder, string projectName) =>
        builder
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", projectName))
            .AddAppSettings();

    /// <summary>
    ///     Add the following to the configuration builder:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>appsettings.json</description>
    ///         </item>
    ///         <item>
    ///             <description>appsettings.{environment}.json</description>
    ///         </item>
    ///         <item>
    ///             <description>environment variables</description>
    ///         </item>
    ///     </list>
    /// </summary>
    /// <param name="builder">The configuration builder.</param>
    /// <returns>The updated configuration builder.</returns>
    public static IConfigurationBuilder AddAppSettings(this IConfigurationBuilder builder) =>
        builder
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{AspNetEnv.Get}.json", true, true)
            .AddEnvironmentVariables();

    /// <summary>
    ///     Get the connection string from the provided WebApplicationBuilder.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder.</param>
    /// <param name="target">The target connection string.</param>
    /// <returns>The connection string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no connection string is found.</exception>
    public static string GetConnectionString(this WebApplicationBuilder builder, string target = "Default") =>
        builder.Configuration.GetConnectionString(target) ??
        throw new InvalidOperationException(
            "Connection string is not set in appsettings files or environment variables"
        );

    /// <summary>
    ///     Try to get the connection string from the provided application arguments.
    /// </summary>
    /// <param name="args">The application arguments.</param>
    /// <returns>The connection string or null.</returns>
    public static string? GetConnectionString(string[]? args) =>
        args is not null && args.Length > 0 ? args[0] : null;


    /// <summary>
    ///     Get the connection string from the provided project appsettings files.
    /// </summary>
    /// <param name="projectName">The name of the project.</param>
    /// <param name="target">The target connection string.</param>
    /// <returns>The connection string.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no connection string is found.</exception>
    public static string GetExternalConnectionString(string projectName, string target = "Default") =>
        new ConfigurationBuilder().AddProjectSettings(projectName).Build().GetConnectionString(target)
        ?? throw new InvalidOperationException($"No connection string found in {projectName} appsettings files");
}