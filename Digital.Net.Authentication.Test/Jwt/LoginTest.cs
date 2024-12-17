using System.Net;
using Digital.Net.Authentication.Options;
using Digital.Net.Entities.Repositories;
using Digital.Net.TestTools.Integration;
using InternalTestProgram;
using InternalTestProgram.Controllers;
using InternalTestProgram.Extensions;
using InternalTestProgram.Factories;
using InternalTestProgram.Models;

namespace Digital.Net.Authentication.Test.Jwt;

public class LoginApiTest : IntegrationTest<Program, TestContext>
{
    private readonly TestUserFactory _testUserFactory;

    public LoginApiTest(AppFactory<Program, TestContext> fixture) : base(fixture)
    {
        _testUserFactory = new TestUserFactory(new Repository<TestUser>(GetContext()));
    }

    [Fact]
    public async Task Login_ShouldReturnToken()
    {
        var (user, password) = _testUserFactory.Create();
        var response = await BaseClient.Login(user.Login, password);
        var content = await response.Content.ReadAsStringAsync();
        var refreshToken = response.Headers.GetValues("Set-Cookie").First();
        Assert.IsType<string>(content);
        Assert.NotNull(refreshToken);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorizedOnWrongPassword()
    {
        var (user, _) = _testUserFactory.Create();
        var response = await BaseClient.Login(user.Login, "wrongPassword");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorizedOnInactiveUser()
    {
        var (user, password) = _testUserFactory.Create(new NullableTestUser { IsActive = false });
        var response = await BaseClient.Login(user.Login, password);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldAllowOnlyXTokenPerUser()
    {
        var (user, password) = _testUserFactory.Create();
        var responses = new List<HttpResponseMessage>();

        CreateClient(AuthenticationDefaults.MaxConcurrentSessions + 1);
        foreach (var client in Clients)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd($"Client {Clients.IndexOf(client)}");
            await client.Login(user.Login, password);
            responses.Add(await client.GetAsync(TestAuthorizeController.TestApiKeyOrJwtRoute));
        }

        var unauthorizedResponse = await BaseClient.RefreshTokens();
        foreach (var response in responses)
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorizedResponse.StatusCode);
    }
}