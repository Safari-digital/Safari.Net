using System.Security.Authentication;

namespace Digital.Net.Authentication.Exceptions;

public class ExpiredApiKeyException() : AuthenticationException("Api key is expired");