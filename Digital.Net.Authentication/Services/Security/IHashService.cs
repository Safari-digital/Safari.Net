namespace Digital.Net.Authentication.Services.Security;

/// <summary>
///     Service for hash/verify passwords and API keys.
/// </summary>
public interface IHashService
{
    /// <summary>
    ///     Hashes a password.
    /// </summary>
    /// <param name="password"> The password to hash. </param>
    /// <returns> The hashed password. </returns>
    public string HashPassword(string password);

    /// <summary>
    ///     Validate a clear password using a Regex.
    /// </summary>
    /// <param name="password"> The password to validate. </param>
    /// <returns> True if the password is valid, false otherwise. </returns>
    public bool VerifyPasswordValidity(string password);
}