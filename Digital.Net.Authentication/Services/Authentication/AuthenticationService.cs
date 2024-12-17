using Digital.Net.Authentication.Exceptions;
using Digital.Net.Authentication.Models;
using Digital.Net.Authentication.Services.Authentication.ApiUsers;
using Digital.Net.Authentication.Services.Authentication.Events;
using Digital.Net.Authentication.Services.Authorization;
using Digital.Net.Authentication.Services.Options;
using Digital.Net.Authentication.Services.Security;
using Digital.Net.Core.Messages;
using Digital.Net.Entities.Models;
using Digital.Net.Entities.Repositories;
using Digital.Net.Mvc.Services;
using Microsoft.EntityFrameworkCore;

namespace Digital.Net.Authentication.Services.Authentication;

public class AuthenticationService<TApiUser>(
    IHttpContextService httpContextService,
    IHashService hashService,
    IJwtOptionService jwtOptions,
    IAuthenticationEventService<TApiUser> authenticationEventService,
    IAuthenticationJwtService authenticationJwtService,
    IAuthorizationJwtService<TApiUser> authorizationJwtService,
    IApiUserService<TApiUser> apiUserService,
    IRepository<TApiUser> apiUserRepository
) : IAuthenticationService<TApiUser>
    where TApiUser : EntityGuid, IApiUser
{
    public async Task<Result<TApiUser>> ValidateCredentials(string login, string password)
    {
        var result = new Result<TApiUser>
        {
            Value = await apiUserRepository.Get(u => u.Login == login).FirstOrDefaultAsync()
        };

        if (result.Value is null)
            result.AddError(new AuthenticationInvalidCredentialsException());
        else if (!result.Value.IsActive)
            result.AddError(new AuthenticationInactiveUserException());
        else if (!HashService.VerifyPassword(result.Value, password))
            result.AddError(new AuthenticationInvalidCredentialsException());

        return result;
    }

    public async Task<Result<string>> Login(string login, string password)
    {
        AuthenticationEventType eventType;
        var result = new Result<string>();
        var userResult = new Result<TApiUser>();

        if (authenticationEventService.HasTooManyAttempts(login))
        {
            result.AddError(new AuthenticationTooManyAttemptsException());
            eventType = AuthenticationEventType.LoginTooManyRequests;
        }
        else
        {
            userResult = await ValidateCredentials(login, password);
            eventType = userResult.HasError
                ? AuthenticationEventType.LoginFailure
                : AuthenticationEventType.LoginSuccess;
        }

        result.Merge(userResult);

        await authenticationEventService.RegisterEventAsync(eventType, result, userResult.Value?.Id, login);

        if (result.HasError)
            return result;

        result.Value = authenticationJwtService.GenerateBearerToken(userResult.Value!.Id);
        httpContextService.SetResponseCookie(
            authenticationJwtService.GenerateRefreshToken(userResult.Value.Id),
            jwtOptions.CookieName,
            jwtOptions.GetRefreshTokenExpirationDate()
        );
        return result;
    }

    public Result<string> RefreshTokens()
    {
        var cookieName = jwtOptions.CookieName;
        var token = httpContextService.Request.Cookies[cookieName];
        var result = new Result<string>();

        var tokenResult = authorizationJwtService.AuthorizeApiUserRefresh(token);
        result.Merge(tokenResult);
        if (result.HasError)
            return result;

        httpContextService.SetResponseCookie(
            authenticationJwtService.GenerateRefreshToken(tokenResult.ApiUserId),
            cookieName,
            jwtOptions.GetRefreshTokenExpirationDate()
        );
        result.Value = authenticationJwtService.GenerateBearerToken(tokenResult.ApiUserId);
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

        await authenticationEventService.RegisterEventAsync(
            AuthenticationEventType.Logout,
            null,
            apiUserService.GetAuthenticatedUserId()
        );
    }

    public async Task LogoutAll()
    {
        var apiUserId = apiUserService.GetAuthenticatedUserId();
        if (apiUserId is null)
            return;

        await authenticationJwtService.RevokeAllTokensAsync((Guid)apiUserId);
        httpContextService.Response.Cookies.Delete(jwtOptions.CookieName);

        await authenticationEventService.RegisterEventAsync(
            AuthenticationEventType.LogoutAll,
            null,
            apiUserId
        );
    }
}