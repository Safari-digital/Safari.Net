using System.Net;
using Digital.Net.Authentication.Options;
using Digital.Net.Authentication.Services.Authentication.Events;
using Digital.Net.Core.Extensions.StringUtilities;
using Digital.Net.Entities.Repositories;
using Digital.Net.Http.HttpClient.Extensions;
using Digital.Net.TestTools.Integration;
using InternalTestProgram;
using InternalTestProgram.Controllers;
using InternalTestProgram.Extensions;
using InternalTestProgram.Factories;
using InternalTestProgram.Models;

namespace Digital.Net.Authentication.Test.Jwt;

public class LoginIntegrationTest : IntegrationTest<Program, TestContext>
{
    private readonly TestUserFactory _testUserFactory;
    private readonly Repository<AuthEvent> _authEventRepository;
    private readonly Repository<ApiToken> _apiTokenRepository;

    public LoginIntegrationTest(AppFactory<Program, TestContext> fixture) : base(fixture)
    {
        _testUserFactory = new TestUserFactory(new Repository<TestUser>(GetContext()));
        _authEventRepository = new Repository<AuthEvent>(GetContext());
        _apiTokenRepository = new Repository<ApiToken>(GetContext());
    }

    [Fact]
    public async Task Login_OnSuccess()
    {
        var (user, password) = _testUserFactory.Create();
        var response = await BaseClient.Login(user.Login, password);
        var registeredToken = _apiTokenRepository.Get(x => x.ApiUserId == user.Id).First();
        var record = _authEventRepository.Get(x => x.ApiUserId == user.Id).First();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(registeredToken);
        Assert.True(record.EventType == AuthenticationEventType.LoginSuccess);
        Assert.True((await response.Content.ReadAsStringAsync()).IsJsonWebToken());
        Assert.True(response.Headers.TryGetCookie("Cookie")?.IsJsonWebToken());
    }

    [Fact]
    public async Task Login_OnWrongPassword()
    {
        var (user, _) = _testUserFactory.Create();
        var response = await BaseClient.Login(user.Login, "wrongPassword");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_OnInactiveUser()
    {
        var (user, password) = _testUserFactory.Create(new NullableTestUser { IsActive = false });
        var response = await BaseClient.Login(user.Login, password);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_OnMaxCurrentSessions()
    {
        var (user, password) = _testUserFactory.Create();
        var responses = new List<HttpResponseMessage>();

        CreateClient(AuthenticationDefaults.MaxConcurrentSessions);
        foreach (var client in Clients)
        {
            // client.DefaultRequestHeaders.UserAgent.ParseAdd($"Client {Clients.IndexOf(client)}");
            await client.Login(user.Login, password);
            responses.Add(await client.GetAsync(TestAuthorizeController.TestApiKeyOrJwtRoute));
        }

        var unauthorizedResponse = await BaseClient.RefreshTokens();
        foreach (var response in responses)
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorizedResponse.StatusCode);
    }
}