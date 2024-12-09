using System.Security.Authentication;

namespace Digital.Net.Authentication.Exceptions;

public class UnExistingApiKeyException() : AuthenticationException("Api key does not exist");