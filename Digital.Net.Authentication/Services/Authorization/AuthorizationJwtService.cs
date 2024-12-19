using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Digital.Net.Authentication.Exceptions;
using Digital.Net.Authentication.Models;
using Digital.Net.Authentication.Models.Authorizations;
using Digital.Net.Authentication.Services.Options;
using Digital.Net.Entities.Models;
using Digital.Net.Entities.Repositories;
using Digital.Net.Mvc.Services;

namespace Digital.Net.Authentication.Services.Authorization;

public class AuthorizationJwtService<TApiUser, TAuthorization>(
    IRepository<TApiUser> apiUserRepository,
    IRepository<TAuthorization> authorizationRepository,
    IHttpContextService httpContextService,
    IJwtOptionService jwtOptionService
) : IAuthorizationJwtService<TApiUser>
    where TApiUser : EntityGuid, IApiUser
    where TAuthorization : AuthorizationToken, new()
{
    public string? GetRequestKey() => httpContextService.BearerToken;

    public AuthorizationResult AuthorizeApiUser(string? token)
    {
        var result = new AuthorizationResult();
        if (string.IsNullOrWhiteSpace(token))
            return result.AddError(new AuthorizationTokenNotFoundException());

        var handler = new JwtSecurityTokenHandler();
        try
        {
            handler.ValidateToken(token, jwtOptionService.GetTokenParameters(), out _);
            var content = handler
                .ReadJwtToken(token)
                .Claims.First(c => c.Type == JwtOptionService.ContentClaimType)
                .Value;
            var decoded = JsonSerializer.Deserialize<TokenContent>(content);
            var apiUser = apiUserRepository.Get(u => decoded != null && u.Id == decoded.Id).FirstOrDefault();

            if (apiUser is null)
                throw new AuthorizationInvalidTokenException();

            result.ApiUserId = apiUser.Id;
        }
        catch (Exception e)
        {
            result.AddError(e);
        }

        return result;
    }

    public AuthorizationResult AuthorizeApiUserRefresh(string? token)
    {
        var record = authorizationRepository
            .Get(a => a.Key == (token ?? string.Empty))
            .FirstOrDefault();
        return record is null
            ? new AuthorizationResult().AddError(new AuthorizationInvalidTokenException())
            : AuthorizeApiUser(record.Key);
    }
}