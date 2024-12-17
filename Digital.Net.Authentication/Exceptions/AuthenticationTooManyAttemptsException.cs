using System.Security.Authentication;

namespace Digital.Net.Authentication.Exceptions;

public class AuthenticationTooManyAttemptsException() : AuthenticationException("Too many login attempts");