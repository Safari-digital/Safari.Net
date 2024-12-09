using Digital.Net.Authentication.Extensions;
using Digital.Net.Authentication.Models;
using Digital.Net.Authentication.Services;
using Digital.Net.Core.Errors;
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
        var result = new Result();

        if (Type.HasFlag(AuthorizeType.ApiKey))
            result.Merge(AuthorizeApiKey(context));
        if (Type.HasFlag(AuthorizeType.Jwt))
            result.Merge(AuthorizeJwt(context));
        if (!result.HasError)
            return;

        OnAuthorizationFailure(context, result);
        context.SetUnauthorisedResult();
    }

    private Result AuthorizeApiKey(AuthorizationFilterContext context)
    {
        var result = new Result();
        var service = TryCatchUtilities.TryOrNull(
            () => context.HttpContext.RequestServices.GetRequiredService<IApiKeyService>()
        );
        if (service is null)
            throw new ApplicationException("DigitalApiKeyAuthorization as not been added to the service collection.");

        var apiKey = service.GetApiKey();
        result.Try(() => OnApiKeyAuthorization(context));

        if (Type.HasFlag(AuthorizeType.Jwt) && (apiKey is null || result.HasError))
            new Result().AddInfo("JWT Authorization available. Continue with JWT Authorization.");
        else if (result.HasError)
            return result;

        return service.ValidateApiKey(apiKey);
    }

    private Result AuthorizeJwt(AuthorizationFilterContext context) => throw new NotImplementedException();

    /// <summary>
    ///     Executes custom logic for API Key authorization.
    /// </summary>
    /// <param name="context">The context of the authorization.</param>
    /// <remarks>Override this method to execute custom logic during API Key authorization.</remarks>
    /// <remarks>Throw an exception to return an unauthorized result.</remarks>
    protected static void OnApiKeyAuthorization(AuthorizationFilterContext context)
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