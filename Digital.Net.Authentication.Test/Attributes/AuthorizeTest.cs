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
    private readonly Repository<ApiKey> _apiKeyRepository;

    public AuthorizeTest(AppFactory<Program, TestContext> fixture) : base(fixture)
    {
        _apiKeyRepository = new Repository<ApiKey>(GetContext());
    }

    private void Setup(DateTime? expiredAt = null)
    {
        var apiKey = new ApiKey(null, expiredAt);
        _apiKeyRepository.Create(apiKey);
        _apiKeyRepository.Save();
        BaseClient.DefaultRequestHeaders.Add(DefaultHeaders.ApiKeyHeader, apiKey.Key);
    }

    [Fact]
    public async Task Authorize_ShouldReturnOk()
    {
        Setup();
        var response = await BaseClient.GetAsync(TestAuthenticationController.TestApiKeyRoute);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Authorize_ShouldReturnOkOnMultipleAuthorizationTypeRoutes()
    {
        Setup();
        var response = await BaseClient.GetAsync(TestAuthenticationController.TestApiKeyOrJwtRoute);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Authorize_ShouldReturnUnauthorizedOnMissingApiKeyHeader()
    {
        var response = await BaseClient.GetAsync(TestAuthenticationController.TestApiKeyRoute);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Authorize_ShouldReturnUnauthorizedOnExpiredApiKey()
    {
        Setup(DateTime.UtcNow.AddHours(-1));
        var response = await BaseClient.GetAsync(TestAuthenticationController.TestApiKeyRoute);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Authorize_ShouldReturnOkOnUnexpiredApiKey()
    {
        Setup(DateTime.UtcNow.AddHours(1));
        var response = await BaseClient.GetAsync(TestAuthenticationController.TestApiKeyRoute);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}