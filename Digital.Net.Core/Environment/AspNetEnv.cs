namespace Digital.Net.Core.Environment;

/// <summary>
///     A class to get the current environment of the application.
/// </summary>
public static class AspNetEnv
{
    /// <summary>
    ///     Get the current environment of the application.
    /// </summary>
    public static string Get =>
        System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

    /// <summary>
    ///     Check if the current environment is "Test".
    /// </summary>
    public static bool IsTest => Get == "Test";

    /// <summary>
    ///     Check if the current environment is "Development".
    /// </summary>
    public static bool IsDevelopment => Get == "Development";

    /// <summary>
    ///     Check if the current environment is "Production".
    /// </summary>
    public static bool IsProduction => Get == "Production";

    /// <summary>
    ///     Check if the current environment is "Staging".
    /// </summary>
    public static bool IsStaging => Get == "Staging";

    /// <summary>
    ///     Check if the current environment equals the given environment.
    /// </summary>
    /// <param name="environment">The environment to check against.</param>
    public static bool Is(string environment) => Get == environment;
}