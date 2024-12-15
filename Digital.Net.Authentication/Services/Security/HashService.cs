using System.Security.Cryptography;
using System.Text;
using Digital.Net.Authentication.Models;
using Digital.Net.Authentication.Models.Authorizations;
using Digital.Net.Authentication.Services.Options;

namespace Digital.Net.Authentication.Services.Security;

public class HashService(IJwtOptionService options) : IHashService
{
    /// <summary>
    ///     Verifies the password against the stored hash.
    /// </summary>
    /// <param name="user">
    ///     The user to verify the password against. The password hash is stored in the user object.
    /// </param>
    /// <param name="password">The password to verify.</param>
    /// <returns>True if the password is correct, false otherwise.</returns>
    public static bool VerifyPassword(IApiUser user, string password) =>
        BCrypt.Net.BCrypt.Verify(password, user.Password);

    public string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(options.SaltSize));

    public bool VerifyPasswordValidity(string password) => options.PasswordRegex.IsMatch(password);

    /// <summary>
    ///     Verifies the API key against the stored hash.
    /// </summary>
    /// <param name="authorization">
    ///     The authorization object to verify the key against. The key hash is stored in the object.
    /// </param>
    /// <param name="inputKey">
    ///     The key to verify.
    /// </param>
    /// <returns></returns>
    public static bool VerifyApiKey(IAuthorizationKey authorization, string inputKey) =>
        authorization.Key == HashApiKey(inputKey);

    /// <summary>
    ///     Hashes the API key using SHA256.
    /// </summary>
    /// <param name="apiKey"> The key to hash.</param>
    /// <returns></returns>
    public static string HashApiKey(string apiKey)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(apiKey));
        return Convert.ToBase64String(hashBytes);
    }
}