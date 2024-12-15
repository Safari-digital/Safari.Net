using System.Text;
using System.Text.RegularExpressions;
using Digital.Net.Authentication.Options.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Digital.Net.Authentication.Services.Options;

public class JwtOptionService(IOptions<JwtAuthenticationOptions> options) : IJwtOptionService
{
    public const string ContentClaimType = "Content";

    public Regex PasswordRegex => options.Value.PasswordOptions.PasswordRegex;
    public int SaltSize => options.Value.PasswordOptions.SaltSize;
    public int MaxConcurrentSessions => options.Value.JwtTokenOptions.ConcurrentSessions;
    public int MaxLoginAttempts => options.Value.LoginAttemptsOptions.AttemptsThreshold;
    public string CookieName => options.Value.JwtTokenOptions.CookieName;

    public TimeSpan GetLoginAttemptThreshold(DateTime? from = null) =>
        TimeSpan.FromMilliseconds(options.Value.LoginAttemptsOptions.AttemptsThresholdTime);

    public DateTime GetRefreshTokenExpirationDate(DateTime? from = null) =>
        (from ?? DateTime.UtcNow).AddMilliseconds(options.Value.JwtTokenOptions.RefreshTokenExpiration);

    public DateTime GetBearerTokenExpirationDate(DateTime? from = null) =>
        (from ?? DateTime.UtcNow).AddMilliseconds(options.Value.JwtTokenOptions.AccessTokenExpiration);

    public TokenValidationParameters GetTokenParameters() => new()
    {
        ValidateIssuerSigningKey = true,
        ValidIssuer = options.Value.JwtTokenOptions.Issuer,
        ValidAudience = options.Value.JwtTokenOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(options.Value.JwtTokenOptions.Secret)),
        ClockSkew = TimeSpan.Zero
    };
}