namespace Digital.Net.Authentication.Options.Jwt;

public class LoginAttemptsOptions
{
    /// <summary>
    ///     The number of attempts allowed before the user is locked out.
    /// </summary>
    public int AttemptsThreshold { get; set; } = AuthenticationDefaults.MaxLoginAttempts;

    /// <summary>
    ///     The time in milliseconds that the user must wait before the attempts are reset.
    ///     Default is 15 minutes.
    /// </summary>
    public long AttemptsThresholdTime { get; set; } = 900000;
}