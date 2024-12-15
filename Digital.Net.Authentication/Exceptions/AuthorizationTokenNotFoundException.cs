using System.Security.Authentication;

namespace Digital.Net.Authentication.Exceptions;

public class AuthorizationTokenNotFoundException()
    : AuthenticationException("Token could not be found in the request headers");
