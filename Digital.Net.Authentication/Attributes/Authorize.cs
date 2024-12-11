using Digital.Net.Authentication.Extensions;
using Digital.Net.Authentication.Models;
using Digital.Net.Authentication.Services;
using Digital.Net.Core.Messages;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Digital.Net.Authentication.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class AuthorizeAttribute(AuthorizeType type) : Attribute, IAuthorizationFilter
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
            return;

        OnAuthorizationFailure(context, result);
        context.SetUnauthorisedResult();
    }

    private AuthorizationResult AuthorizeApiKey(AuthorizationFilterContext context)
    {
        var result = new AuthorizationResult();
        var service = context.HttpContext.RequestServices.GetRequiredService<IApiKeyService>();

        var apiKey = service.GetApiKey();
        result.Try(() => OnApiKeyAuthorization(context, apiKey));

        if (Type.HasFlag(AuthorizeType.Jwt) && (apiKey is null || result.HasError))
            new Result().AddInfo("JWT Authorization available. Continue with JWT Authorization.");
        else if (result.HasError)
            return result;

        result.Merge(service.ValidateApiKey(apiKey));
        if (!result.HasError)
            result.Authorize();

        return result;
    }

    private AuthorizationResult AuthorizeJwt(AuthorizationFilterContext context) => throw new NotImplementedException();

    /// <summary>
    ///     Executes custom logic for API Key authorization.
    /// </summary>
    /// <param name="context">The context of the authorization.</param>
    /// <param name="apiKey">The API Key to authorize.</param>
    /// <remarks>Override this method to execute custom logic during API Key authorization.</remarks>
    /// <remarks>Throw an exception to return an unauthorized result.</remarks>
    protected static void OnApiKeyAuthorization(AuthorizationFilterContext context, string? apiKey)
    {
    }

    /// <summary>
    ///     Executes custom logic for JWT authorization.
    /// </summary>
    /// <param name="context">The context of the authorization.</param>
    /// <remarks>Override this method to execute custom logic during JWT authorization.</remarks>
    /// <remarks>Throw an exception to return an unauthorized result.</remarks>
    protected static void OnJwtAuthorization(AuthorizationFilterContext context)
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