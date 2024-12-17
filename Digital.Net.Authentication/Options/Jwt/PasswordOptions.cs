using System.Text.RegularExpressions;

namespace Digital.Net.Authentication.Options.Jwt;

public class PasswordOptions
{
    /// <summary>
    ///     The regex to validate the password.
    ///     Default to "^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\\da-zA-Z]).{12,128}$"
    ///     Which means: At least one lowercase, one uppercase, one digit, one special character and between 12 and 128
    ///     characters.
    /// </summary>
    public Regex PasswordRegex { get; set; } = AuthenticationDefaults.PasswordRegex;

    /// <summary>
    ///     The size of the salt to be used in the password hashing.
    ///     Default to 10.
    /// </summary>
    public int SaltSize { get; set; } = AuthenticationDefaults.SaltSize;
}