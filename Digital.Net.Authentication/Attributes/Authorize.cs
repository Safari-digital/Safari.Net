using Digital.Net.Authentication.Exceptions;
using Digital.Net.Authentication.Extensions;
using Digital.Net.Authentication.Models;
using Digital.Net.Authentication.Models.Authorizations;
using Digital.Net.Authentication.Options;
using Digital.Net.Authentication.Services.Authorization;
using Digital.Net.Core.Messages;
using Digital.Net.Entities.Models;
using Digital.Net.Mvc.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Digital.Net.Authentication.Attributes;

/// <summary>
///     Used to authorize an "API user". The authorization can be done using an API Key, a JWT token, or both.
///     The authorization result is stored in the Api Context.
/// </summary>
/// <param name="type">The type of authorization to use.</param>
/// <typeparam name="TApiUser">
///     The type of the API user. The provided type is used to define the correct Repository to look for the user.
///     Must implement <see cref="EntityGuid" /> and <see cref="IApiUser" />.
/// </typeparam>
/// <remarks>
///     An Api user should only use a GUID as an identifier. Thus, you can register as many API users types as you want.
/// </remarks>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizeAttribute<TApiUser>(AuthorizeType type) : Attribute, IAuthorizationFilter
    where TApiUser : EntityGuid, IApiUser
{
    private AuthorizeType Type { get; } = type;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var result = new AuthorizationResult();

        if (Type.HasFlag(AuthorizeType.ApiKey))
            result.Merge(AuthorizeApiKey(context));
        if (Type.HasFlag(AuthorizeType.Jwt) && result.IsAuthorized is false)
            result.Merge(AuthorizeJwt(context));
        if (!result.HasError || result.IsAuthorized)
        {
            var contextService = context.HttpContext.RequestServices.GetRequiredService<IHttpContextService>();
            contextService.AddItem(AuthenticationDefaults.ApiContextAuthorizationKey, result);
            return;
        }

        OnAuthorizationFailure(context, result);
        context.SetUnauthorisedResult();
    }

    private AuthorizationResult AuthorizeApiKey(AuthorizationFilterContext context)
    {
        var result = new AuthorizationResult();
        var service = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationApiKeyService<TApiUser>>();
        var apiKey = service.GetRequestKey();

        if (Type.HasFlag(AuthorizeType.Jwt) && apiKey is null)
            return result;

        result.Merge(service.AuthorizeApiUser(apiKey));
        result.Try(() => OnApiKeyAuthorization(context, apiKey, result.ApiUserId));
        if (!result.HasError)
            result.Authorize();

        return result;
    }

    private AuthorizationResult AuthorizeJwt(AuthorizationFilterContext context)
    {
        var result = new AuthorizationResult();
        var service = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationJwtService<TApiUser>>();
        var token = service.GetRequestKey();

        if (token is null)
            return result.AddError(new AuthorizationTokenNotFoundException());

        result.Merge(service.AuthorizeApiUser(token));
        result.Try(() => OnJwtAuthorization(context, token, result.ApiUserId));
        if (!result.HasError)
            result.Authorize();

        return result;
    }

    /// <summary>
    ///     Executes custom logic for API Key authorization.
    /// </summary>
    /// <param name="context">The context of the authorization.</param>
    /// <param name="apiKey">The API Key to authorize.</param>
    /// <remarks>Override this method to execute custom logic during API Key authorization.</remarks>
    /// <remarks>Throw an exception to return an unauthorized result.</remarks>
    protected static void OnApiKeyAuthorization(AuthorizationFilterContext context, string? apiKey, Guid? apiUserId)
    {
    }

    /// <summary>
    ///     Executes custom logic for JWT authorization.
    /// </summary>
    /// <param name="context">The context of the authorization.</param>
    /// <remarks>Override this method to execute custom logic during JWT authorization.</remarks>
    /// <remarks>Throw an exception to return an unauthorized result.</remarks>
    protected static void OnJwtAuthorization(AuthorizationFilterContext context, string? token, Guid? apiUserId)
    {
    }

    /// <summary>
    ///     Executes custom logic for authorization failure.
    /// </summary>
    /// <param name="context">The context of the authorization.</param>
    /// <param name="result">The result of the authorization.</param>
    /// <remarks>Override this method to execute custom logic when authorization fails.</remarks>
    protected static void OnAuthorizationFailure(AuthorizationFilterContext context, Result result)
    {
    }
}