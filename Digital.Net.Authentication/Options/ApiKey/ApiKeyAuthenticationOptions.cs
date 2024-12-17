namespace Digital.Net.Authentication.Options.ApiKey;

public class ApiKeyAuthenticationOptions
{
    public string HeaderAccessor { get; private set; } = AuthenticationDefaults.ApiKeyHeader;

    /// <summary>
    ///     Define the header key to access the API key from the request.
    /// </summary>
    /// <param name="headerKey">The header key to access the API key from the request.</param>
    /// <returns></returns>
    public ApiKeyAuthenticationOptions SetHeaderAccessor(string headerKey)
    {
        HeaderAccessor = headerKey;
        return this;
    }
}