using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Digital.Net.Entities.Models;

namespace Digital.Net.Authentication.Models.Authorizations;

public abstract class AuthorizationToken : EntityId, IAuthorizationKey
{
    [Column("Key"), Required, MaxLength(1024)]
    public string Key { get; init; } = string.Empty;

    [Column("UserAgent"), Required, MaxLength(1024)]
    public string UserAgent { get; init; } = string.Empty;

    [Column("ApiUserId"), Required]
    public Guid ApiUserId { get; init; }

    [Column("ExpiredAt"), Required]
    public DateTime? ExpiredAt { get; set; }
}