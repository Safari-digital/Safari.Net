using System.Text.RegularExpressions;

namespace Digital.Net.Authentication.Options;

public static partial class Defaults
{
    public const string ApiKeyHeader = "API-Key";
    public const string ApiContextAuthorizationKey = "AuthorizationResult";

    public static readonly Regex PasswordRegex = PRegex();

    [GeneratedRegex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{12,128}$")]
    private static partial Regex PRegex();
}