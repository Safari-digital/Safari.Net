using Digital.Net.Authentication.Options;
using Digital.Net.Authentication.Services.Security;
using Digital.Net.Core.Random;
using Digital.Net.Entities.Repositories;
using Digital.Net.TestTools.Data.Factories;
using InternalTestProgram.Models;

namespace InternalTestProgram.Factories;

public class TestUserFactory(IRepository<TestUser> repository)
{
    private readonly DataFactory<TestUser> _userFactory = new(repository);
    public void Dispose() => _userFactory.Dispose();

    public (TestUser, string) Create(NullableTestUser? userPayload = null)
    {
        var password = Randomizer.GenerateRandomString();
        var user = Build();
        if (userPayload is not null)
            user.Update(userPayload);

        user.Update(new NullableTestUser { Password = HashService.HashPassword(password) });
        var created = _userFactory.Create(user);
        return (created, password);
    }

    public List<TestUser> CreateMany(int count)
    {
        var users = Enumerable.Range(0, count).Select(_ => Build()).ToList();
        return users.Select(user => _userFactory.Create(user)).ToList();
    }

    private static TestUser Build() => new()
    {
        Username = Randomizer.GenerateRandomString(),
        Password = HashService.HashPassword(Randomizer.GenerateRandomString(null, 64), AuthenticationDefaults.SaltSize),
        Email = Randomizer.GenerateRandomEmail(),
        Role = new TestRole(),
        IsActive = true
    };
}