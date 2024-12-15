using System.Security.Authentication;

namespace Digital.Net.Authentication.Exceptions;

public class AuthorizationExpiredTokenException() : AuthenticationException("Token is expired");