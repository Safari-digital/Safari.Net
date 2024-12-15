using System.Security.Authentication;

namespace Digital.Net.Authentication.Exceptions;

public class AuthorizationInvalidTokenException() : AuthenticationException("Invalid Token.");