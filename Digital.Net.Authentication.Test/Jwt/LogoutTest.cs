using System.Net;
using Digital.Net.Entities.Repositories;
using Digital.Net.TestTools.Integration;
using InternalTestProgram;
using InternalTestProgram.Extensions;
using InternalTestProgram.Factories;
using InternalTestProgram.Models;

namespace Digital.Net.Authentication.Test.Jwt;

// TODO: Test refresh token api + Logout All
// TODO: Test AuthEvents registration
public class LogoutTest : IntegrationTest<Program, TestContext>
{
    private readonly TestUserFactory _testUserFactory;
    private readonly IRepository<ApiToken> _apiTokenRepository;

    public LogoutTest(AppFactory<Program, TestContext> fixture) : base(fixture)
    {
        _testUserFactory = new TestUserFactory(new Repository<TestUser>(GetContext()));
        _apiTokenRepository = new Repository<ApiToken>(GetContext());
    }

    [Fact]
    public async Task Logout_ShouldLogoutClient()
    {
        var (user, password) = _testUserFactory.Create();
        await BaseClient.Login(user.Login, password);
        Assert.True(await _apiTokenRepository.CountAsync(x => x.ApiUserId == user.Id) == 1);

        var logoutResponse = await BaseClient.Logout();
        Assert.True(_apiTokenRepository.Get(x => x.ApiUserId == user.Id).First().ExpiredAt <= DateTime.UtcNow);
        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);
    }
}