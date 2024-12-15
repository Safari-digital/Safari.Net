using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;

namespace Digital.Net.Authentication.Services.Options;

public interface IJwtOptionService
{
    public Regex PasswordRegex { get; }
    public int SaltSize { get; }
    public string CookieName { get; }
    public int MaxConcurrentSessions { get; }
    public int MaxLoginAttempts { get; }
    public TimeSpan GetLoginAttemptThreshold(DateTime? from = null);
    public DateTime GetRefreshTokenExpirationDate(DateTime? from = null);
    public DateTime GetBearerTokenExpirationDate(DateTime? from = null);
    public TokenValidationParameters GetTokenParameters();
}