using System.Net;
using Digital.Net.Authentication.Options;
using Digital.Net.Entities.Repositories;
using Digital.Net.TestTools.Integration;
using InternalTestProgram;
using InternalTestProgram.Controllers;
using InternalTestProgram.Models;

namespace Digital.Net.Authentication.Test.Attributes;

public class AuthorizeTest : IntegrationTest<Program, TestContext>
{
    private readonly Repository<TestUser> _userRepository;
    private readonly Repository<FakeUser> _fakeUserRepository;
    private readonly Repository<ApiKey> _apiKeyRepository;

    public AuthorizeTest(AppFactory<Program, TestContext> fixture) : base(fixture)
    {
        var context = GetContext();
        _apiKeyRepository = new Repository<ApiKey>(context);
        _fakeUserRepository = new Repository<FakeUser>(context);
        _userRepository = new Repository<TestUser>(context);
    }

    private void Setup(DateTime? expiredAt = null)
    {
        var user = new TestUser { Id = Guid.NewGuid() };
        var fakeUser = new FakeUser { Id = Guid.NewGuid() };
        var apiKey = new ApiKey { ApiUserId = user.Id, ExpiredAt = expiredAt };
        _userRepository.Create(user);
        _fakeUserRepository.Create(fakeUser);
        _apiKeyRepository.Create(apiKey);
        _apiKeyRepository.Save();
        BaseClient.DefaultRequestHeaders.Add(Defaults.ApiKeyHeader, apiKey.Key);
    }

    [Fact]
    public async Task Authorize_WithValidApiKey_ShouldReturnOk()
    {
        Setup();
        var response = await BaseClient.GetAsync(TestAuthenticationController.TestApiKeyRoute);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Authorize_WithoutMatchingEntityApiKeyOnMultipleEntityServices_ShouldReturnUnauthorized()
    {
        Setup();
        var response = await BaseClient.GetAsync(TestAuthenticationController.TestFakeUserApiKeyRoute);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Authorize_WithValidApiKey_ShouldReturnOk_OnMultipleAuthorizationTypeRoutes()
    {
        Setup();
        var response = await BaseClient.GetAsync(TestAuthenticationController.TestApiKeyOrJwtRoute);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Authorize_ShouldReturnUnauthorized_OnMissingApiKeyHeader()
    {
        var response = await BaseClient.GetAsync(TestAuthenticationController.TestApiKeyRoute);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}