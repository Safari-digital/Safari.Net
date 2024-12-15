namespace Digital.Net.Authentication.Options.Jwt;

public class JwtAuthenticationOptions
{
    public PasswordOptions PasswordOptions { get; private set; } = new();

    /// <summary>
    ///     Set the password validation and security options.
    /// </summary>
    public JwtAuthenticationOptions SetPasswordOptions(PasswordOptions options)
    {
        PasswordOptions = options;
        return this;
    }

    public LoginAttemptsOptions LoginAttemptsOptions { get; private set; } = new();

    /// <summary>
    ///     Set the login attempts configuration options.
    /// </summary>
    public JwtAuthenticationOptions SetLoginAttemptsOptions(LoginAttemptsOptions options)
    {
        LoginAttemptsOptions = options;
        return this;
    }

    public JwtTokenOptions JwtTokenOptions { get; private set; } = new();

    /// <summary>
    ///     Set the JWT token configuration options.
    /// </summary>
    public JwtAuthenticationOptions SetJwtTokenOptions(JwtTokenOptions options)
    {
        JwtTokenOptions = options;
        return this;
    }
}