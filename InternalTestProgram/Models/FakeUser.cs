using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Digital.Net.Core.Random;
using Digital.Net.Entities.Attributes;
using Digital.Net.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace InternalTestProgram.Models;

[Index(nameof(Username), nameof(Email), IsUnique = true)]
public class FakeUser : EntityWithGuid
{
    [Column("Username"), MaxLength(24), Required, RegexValidation("^[a-zA-Z0-9.'@_-]{6,24}$")]
    public string Username { get; set; } = Randomizer.GenerateRandomString();

    [Column("Password"), MaxLength(64), Required, Secret]
    public string Password { get; set; } = Randomizer.GenerateRandomString();

    [Column("Email"), MaxLength(254), Required, RegexValidation(@"^[^@]+@[^@]+\.[^@]{2,253}$")]
    public string Email { get; set; } = Randomizer.GenerateRandomString();

    [Column("Role"), ReadOnly]
    public virtual FakeRole? Role { get; set; }
}