using System.Security.Authentication;

namespace Digital.Net.Authentication.Exceptions;

public class AuthenticationInvalidCredentialsException() : AuthenticationException("Provided credentials are invalid.");