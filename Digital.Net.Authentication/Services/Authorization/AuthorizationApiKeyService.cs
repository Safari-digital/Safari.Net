using Digital.Net.Authentication.Exceptions;
using Digital.Net.Authentication.Models;
using Digital.Net.Authentication.Models.Authorizations;
using Digital.Net.Authentication.Options.ApiKey;
using Digital.Net.Entities.Models;
using Digital.Net.Entities.Repositories;
using Digital.Net.Mvc.Services;
using Microsoft.Extensions.Options;

namespace Digital.Net.Authentication.Services.Authorization;

public class AuthorizationApiKeyService<TApiUser, TAuthorization>(
    IRepository<TAuthorization> authorizationRepository,
    IRepository<TApiUser> apiUserRepository,
    IHttpContextService httpContextService,
    IOptions<ApiKeyAuthenticationOptions> options
) : IAuthorizationApiKeyService<TApiUser>
    where TApiUser : EntityGuid, IApiUser
    where TAuthorization : AuthorizationApiKey, new()
{
    public string? GetRequestKey() => httpContextService.GetHeaderValue(options.Value.HeaderAccessor);

    public AuthorizationResult AuthorizeApiUser(string? key)
    {
        var result = new AuthorizationResult();
        if (string.IsNullOrWhiteSpace(key))
            return result.AddError(new AuthorizationTokenNotFoundException());

        var authorization = authorizationRepository.Get(k => k.Key == key).FirstOrDefault();
        if (authorization is null)
            return result.AddError(new AuthorizationInvalidTokenException());

        if (authorization.ExpiredAt is not null && authorization.ExpiredAt < DateTime.UtcNow)
            return result.AddError(new AuthorizationExpiredTokenException());

        var apiUser = apiUserRepository.Get(u => u.Id == authorization.ApiUserId).FirstOrDefault();
        if (apiUser is null)
            return result.AddError(new AuthorizationInvalidTokenException());

        result.ApiUserId = apiUser.Id;
        return result;
    }
}