using System.Text.RegularExpressions;

namespace Digital.Net.Authentication.Options;

public static partial class AuthenticationDefaults
{
    public const string ApiKeyHeader = "API-Key";
    public const string ApiContextAuthorizationKey = "AuthorizationResult";

    public const int SaltSize = 12;
    public const int MaxLoginAttempts = 3;
    public const int MaxConcurrentSessions = 5;


    public static readonly Regex PasswordRegex = PRegex();

    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{12,128}$")]
    private static partial Regex PRegex();

}