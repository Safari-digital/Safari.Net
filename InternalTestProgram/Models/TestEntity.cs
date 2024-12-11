using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Digital.Net.Core.Random;
using Digital.Net.Entities.Attributes;
using Digital.Net.Entities.Models;

namespace InternalTestProgram.Models;

public class TestIdEntity : EntityWithId
{
    [Column("Username"), Required]
    public string Username { get; set; } = Randomizer.GenerateRandomString();

    [Column("Password"), Required, Secret]
    public string Password { get; set; } = Randomizer.GenerateRandomString();

    [Column("Email")]
    public string Email { get; set; } = Randomizer.GenerateRandomString();
}

public class TestGuidEntity : EntityWithGuid
{
    [Column("Username"), Required]
    public string Username { get; set; } = Randomizer.GenerateRandomString();

    [Column("Password"), Required, Secret]
    public string Password { get; set; } = Randomizer.GenerateRandomString();

    [Column("Email")]
    public string Email { get; set; } = Randomizer.GenerateRandomString();
}