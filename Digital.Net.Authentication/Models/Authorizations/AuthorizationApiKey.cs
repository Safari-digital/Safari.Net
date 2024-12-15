using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Digital.Net.Authentication.Services.Security;
using Digital.Net.Core.Random;
using Digital.Net.Entities.Attributes;
using Digital.Net.Entities.Models;

namespace Digital.Net.Authentication.Models.Authorizations;

public abstract class AuthorizationApiKey : EntityId, IAuthorizationKey
{
    [Column("Key"), MaxLength(64), Required, ReadOnly]
    public string Key { get; set; } = HashService.HashApiKey(
        Randomizer.GenerateRandomString(Randomizer.AnyLetterOrNumber, 128)
    );

    [Column("ExpiredAt")]
    public DateTime? ExpiredAt { get; set; }

    [Column("ApiUserId"), Required]
    public Guid ApiUserId { get; set; }
}