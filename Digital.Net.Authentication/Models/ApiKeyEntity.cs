using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Digital.Net.Core.Random;
using Digital.Net.Entities.Attributes;
using Digital.Net.Entities.Models;

namespace Digital.Net.Authentication.Models;

public abstract class ApiKeyEntity : EntityWithId
{
    [Column("Key"), MaxLength(128), Required, ReadOnly]
    public string Key { get; private set; }

    [Column("expiredAt")]
    public DateTime? ExpiredAt { get; private set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ApiKeyEntity" /> class.
    /// </summary>
    /// <param name="key">The key to use.</param>
    /// <param name="expiredAt">The date and time the key expires.</param>
    /// <exception cref="ArgumentException">Thrown if the key is not 128 characters long.</exception>
    /// <remarks>If no key is provided, a random key will be generated.</remarks>
    protected ApiKeyEntity(string? key = null, DateTime? expiredAt = null)
    {
        key ??= Randomizer.GenerateRandomString(Randomizer.AnyLetterOrNumber, 128);
        Key = ValidateKey(key);
        ExpiredAt = expiredAt;
    }

    /// <summary>
    ///     Updates the expiration date of the key.
    /// </summary>
    /// <param name="expiredAt">The new expiration date.</param>
    /// <remarks>If no expiration date is provided, the key will never expire.</remarks>
    public void UpdateExpiration(DateTime? expiredAt = null) => ExpiredAt = expiredAt;

    /// <summary>
    ///     Generates a new random key.
    /// </summary>
    protected void GenerateKey() => Key = Randomizer.GenerateRandomString(Randomizer.AnyLetterOrNumber, 128);

    private static string ValidateKey(string key)
    {
        if (key.Length is not 128)
            throw new ArgumentException("Key must be 128 characters long");
        return key;
    }
}