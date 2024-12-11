using System.Security.Authentication;

namespace Digital.Net.Authentication.Exceptions;

public class ApiKeyNotFoundException() : AuthenticationException("Api key could not be found in the request headers")
{
    public static string GetName() => nameof(ApiKeyNotFoundException);
}