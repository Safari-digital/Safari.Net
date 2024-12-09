using Digital.Net.Core.Messages;

namespace Digital.Net.Authentication.Services;

public interface IApiKeyService
{
    /// <summary>
    ///     Get the API key from the headers.
    /// </summary>
    /// <returns>The API key from the headers.</returns>
    public string? GetApiKey();

    /// <summary>
    ///     Validate the API key from the headers.
    /// </summary>
    /// <returns>The result of the validation.</returns>
    public Result ValidateApiKey(string? key);
}