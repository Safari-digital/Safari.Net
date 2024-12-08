using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Digital.Net.Core.Random;
using Digital.Net.Entities.Attributes;
using Digital.Net.Entities.Models;

namespace InternalTestUtilities.Models;

public class TestIdEntity : EntityWithId
{
    [Column("username"), Required]
    public string Username { get; set; } = Randomizer.GenerateRandomString();

    [Column("password"), Required, Secret]
    public string Password { get; set; } = Randomizer.GenerateRandomString();

    [Column("email")]
    public string Email { get; set; } = Randomizer.GenerateRandomString();
}

public class TestGuidEntity : EntityWithGuid
{
    [Column("username"), Required]
    public string Username { get; set; } = Randomizer.GenerateRandomString();

    [Column("password"), Required, Secret]
    public string Password { get; set; } = Randomizer.GenerateRandomString();

    [Column("email")]
    public string Email { get; set; } = Randomizer.GenerateRandomString();
}