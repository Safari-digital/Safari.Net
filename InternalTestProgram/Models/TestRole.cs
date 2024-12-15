using Digital.Net.Core.Random;
using Digital.Net.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace InternalTestProgram.Models;

[Index(nameof(Name), IsUnique = true)]
public class TestRole : EntityId
{
    public string Name { get; set; } = Randomizer.GenerateRandomString();
    public ERole Role { get; set; } = ERole.User;
}

public enum ERole
{
    SuperAdmin,
    Admin,
    User
}