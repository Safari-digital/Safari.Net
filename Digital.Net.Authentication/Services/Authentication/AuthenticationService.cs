using Digital.Net.Authentication.Models;
using Digital.Net.Authentication.Services.Authentication.ApiUsers;
using Digital.Net.Authentication.Services.Authorization;
using Digital.Net.Authentication.Services.Options;
using Digital.Net.Authentication.Services.Security;
using Digital.Net.Core.Messages;
using Digital.Net.Entities.Models;
using Digital.Net.Mvc.Services;

namespace Digital.Net.Authentication.Services.Authentication;

public class AuthenticationService<TApiUser>(
    IHttpContextService httpContextService,
    IHashService hashService,
    IJwtOptionService jwtOptions,
    IAuthenticationJwtService authenticationJwtService,
    IAuthorizationJwtService<TApiUser> jwtAuthorizationService,
    IApiUserService<TApiUser> apiUserService
) : IAuthenticationService<TApiUser>
    where TApiUser : EntityGuid, IApiUser
{
    public const string ApiContextAuthorizationKey = "AuthorizationResult";

    public string GeneratePasswordHash(string password) => hashService.HashPassword(password);

    public async Task<Result<string>> Login(string login, string password)
    {
        var result = new Result<string>();
        // TODO: Implement login logic
        return result;
    }

    public Result<string> RefreshTokens()
    {
        var cookieName = jwtOptions.CookieName;
        var token = httpContextService.Request.Cookies[cookieName];
        var result = new Result<string>();

        var tokenResult = jwtAuthorizationService.AuthorizeApiUserRefresh(token);
        result.Merge(tokenResult);
        if (result.HasError)
            return result;

        var refreshToken = authenticationJwtService.GenerateRefreshToken(tokenResult.ApiUserId);
        var bearerToken = authenticationJwtService.GenerateBearerToken(tokenResult.ApiUserId);

        httpContextService.SetResponseCookie(refreshToken, cookieName, jwtOptions.GetRefreshTokenExpirationDate());
        result.Value = bearerToken;
        return result;
    }

    public async Task Logout()
    {
        var cookieName = jwtOptions.CookieName;
        var refreshToken = httpContextService.Request.Cookies[jwtOptions.CookieName];
        if (refreshToken is null)
            return;
        await authenticationJwtService.RevokeTokenAsync(refreshToken);
        httpContextService.Response.Cookies.Delete(cookieName);
    }

    public async Task LogoutAll()
    {
        var cookieName = jwtOptions.CookieName;
        var apiUserId = apiUserService.GetAuthenticatedUserId();
        if (apiUserId is null)
            return;

        await authenticationJwtService.RevokeAllTokensAsync((Guid)apiUserId);
        httpContextService.Response.Cookies.Delete(cookieName);
    }
}