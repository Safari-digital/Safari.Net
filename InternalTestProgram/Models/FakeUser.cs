using System.ComponentModel.DataAnnotations.Schema;
using Digital.Net.Authentication.Models;
using Digital.Net.Core.Random;
using Digital.Net.Entities.Models;

namespace InternalTestProgram.Models;

[Table("FakeUser")]
public class FakeUser : EntityGuid, IApiUser
{
    public string Password { get; set; } = Randomizer.GenerateRandomString();
    public string Login { get; } = Randomizer.GenerateRandomString();
    public bool IsActive { get; } = true;
}