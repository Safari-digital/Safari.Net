using System.Linq.Expressions;
using Digital.Net.Authentication.Exceptions;
using Digital.Net.Authentication.Options;
using Digital.Net.Authentication.Services;
using Digital.Net.Core.Extensions.ExceptionUtilities;
using Digital.Net.Entities.Repositories;
using Digital.Net.Mvc.Services;
using InternalTestProgram.Models;
using Microsoft.Extensions.Options;
using Moq;

namespace Digital.Net.Authentication.Test.Services;

public class ApiKeyServiceTests
{
    private readonly Mock<IRepository<ApiKey>> _apiKeyRepositoryMock;
    private readonly Mock<IHttpContextService> _httpContextServiceMock;
    private readonly ApiKeyService<ApiKey> _apiKeyService;

    public ApiKeyServiceTests()
    {
        _apiKeyRepositoryMock = new Mock<IRepository<ApiKey>>();
        _httpContextServiceMock = new Mock<IHttpContextService>();

        var optionsMock = new Mock<IOptions<DigitalApiKeyAuthorizationOptions>>();
        optionsMock.Setup(o => o.Value).Returns(new DigitalApiKeyAuthorizationOptions());

        _apiKeyService = new ApiKeyService<ApiKey>(
            _apiKeyRepositoryMock.Object,
            _httpContextServiceMock.Object,
            optionsMock.Object
        );
    }

    [Fact]
    public void GetApiKey_ShouldReturnApiKeyFromHeader()
    {
        const string expectedApiKey = "test-api-key";
        _httpContextServiceMock.Setup(h => h.GetHeaderValue(It.IsAny<string>())).Returns(expectedApiKey);
        Assert.Equal(expectedApiKey, _apiKeyService.GetApiKey());
    }

    [Fact]
    public void ValidateApiKey_ShouldReturnError_WhenApiKeyIsNullOrEmpty()
    {
        var result = _apiKeyService.ValidateApiKey(null);
        Assert.True(result.HasError);
        Assert.Equal(new ApiKeyNotFoundException().GetReference(), result.Errors.First().Reference);
    }

    [Fact]
    public void ValidateApiKey_ShouldReturnError_WhenApiKeyDoesNotExist()
    {
        _apiKeyRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<ApiKey, bool>>>()))
            .Returns(Enumerable.Empty<ApiKey>().AsQueryable());

        var result = _apiKeyService.ValidateApiKey(new ApiKey().Key);
        Assert.True(result.HasError);
        Assert.Equal(new UnExistingApiKeyException().GetReference(), result.Errors.First().Reference);
    }

    [Fact]
    public void ValidateApiKey_ShouldReturnError_WhenApiKeyIsExpired()
    {
        var expiredApiKey = new ApiKey(null, DateTime.UtcNow.AddHours(-1));
        _apiKeyRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<ApiKey, bool>>>()))
            .Returns(new[] { expiredApiKey }.AsQueryable());

        var result = _apiKeyService.ValidateApiKey(expiredApiKey.Key);
        Assert.True(result.HasError);
        Assert.Equal(new ExpiredApiKeyException().GetReference(), result.Errors.First().Reference);
    }

    [Fact]
    public void ValidateApiKey_ShouldReturnSuccess_WhenApiKeyIsValid()
    {
        var validApiKey = new ApiKey(null, DateTime.UtcNow.AddHours(1));
        _apiKeyRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<ApiKey, bool>>>()))
            .Returns(new[] { validApiKey }.AsQueryable());

        var result = _apiKeyService.ValidateApiKey(validApiKey.Key);
        Assert.False(result.HasError);
    }
}