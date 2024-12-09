namespace Digital.Net.Authentication.Options;

public class DigitalApiKeyAuthorizationOptions
{
    public string HeaderAccessor { get; private set; } = DefaultHeaders.ApiKeyHeader;

    /// <summary>
    ///     Define the header key to access the API key from the request.
    /// </summary>
    /// <param name="headerKey">The header key to access the API key from the request.</param>
    /// <returns></returns>
    public DigitalApiKeyAuthorizationOptions SetHeaderAccessor(string headerKey)
    {
        HeaderAccessor = headerKey;
        return this;
    }
}