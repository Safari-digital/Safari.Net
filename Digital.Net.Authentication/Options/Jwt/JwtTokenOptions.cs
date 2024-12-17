namespace Digital.Net.Authentication.Options.Jwt;

public class JwtTokenOptions
{
    /// <summary>
    ///     The issuer of the token.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    ///     The audience of the token.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    ///     The name of the cookie storing the refresh token.
    /// </summary>
    public string CookieName { get; set; } = string.Empty;

    /// <summary>
    ///     The secret key used to sign the token. This should be a long, random string.
    /// </summary>
    /// <remarks>
    ///     This should be at least 32 bytes long. Else it will throw an exception.
    /// </remarks>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    ///     The expiration of the refresh token in milliseconds.
    /// </summary>
    public long RefreshTokenExpiration { get; set; } = 1800000;

    /// <summary>
    ///     The expiration of the access token in milliseconds.
    /// </summary>
    public long AccessTokenExpiration { get; set; } = 300000;

    /// <summary>
    ///     The number of concurrent sessions allowed for a user.
    /// </summary>
    public int ConcurrentSessions { get; set; } = AuthenticationDefaults.MaxConcurrentSessions;
}