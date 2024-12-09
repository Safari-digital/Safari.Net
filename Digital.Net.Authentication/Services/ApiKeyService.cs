using Digital.Net.Authentication.Exceptions;
using Digital.Net.Authentication.Models;
using Digital.Net.Authentication.Options;
using Digital.Net.Core.Messages;
using Digital.Net.Entities.Repositories;
using Digital.Net.Mvc.Services;
using Microsoft.Extensions.Options;

namespace Digital.Net.Authentication.Services;

public class ApiKeyService<T>(
    IRepository<T> apiKeyRepository,
    IHttpContextService httpContextService,
    IOptions<DigitalApiKeyAuthorizationOptions> options
) : IApiKeyService
    where T : ApiKeyEntity
{
    public string? GetApiKey() => httpContextService.GetHeaderValue(options.Value.HeaderAccessor);

    public Result ValidateApiKey(string? key)
    {
        var result = new Result();

        if (string.IsNullOrWhiteSpace(key))
            return result.AddError(new ApiKeyNotFoundException());

        var apiKey = apiKeyRepository.Get(k => k.Key == key).FirstOrDefault();
        if (apiKey is null)
            return result.AddError(new UnExistingApiKeyException());

        if (apiKey.ExpiredAt is not null && apiKey.ExpiredAt < DateTime.UtcNow)
            return result.AddError(new ExpiredApiKeyException());

        return result;
    }
}