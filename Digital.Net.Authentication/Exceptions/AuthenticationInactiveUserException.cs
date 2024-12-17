using System.Security.Authentication;

namespace Digital.Net.Authentication.Exceptions;

public class AuthenticationInactiveUserException() : AuthenticationException("User is inactive");