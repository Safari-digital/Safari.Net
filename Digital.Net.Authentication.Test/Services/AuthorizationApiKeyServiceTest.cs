using System.Linq.Expressions;
using Digital.Net.Authentication.Exceptions;
using Digital.Net.Authentication.Options.ApiKey;
using Digital.Net.Authentication.Services.Authorization;
using Digital.Net.Core.Extensions.ExceptionUtilities;
using Digital.Net.Entities.Repositories;
using Digital.Net.Mvc.Services;
using InternalTestProgram.Models;
using Microsoft.Extensions.Options;
using Moq;

namespace Digital.Net.Authentication.Test.Services;

public class AuthorizationApiKeyServiceTest
{
    private readonly Mock<IRepository<ApiKey>> _apiKeyRepositoryMock;
    private readonly Mock<IRepository<TestUser>> _fakeUserRepositoryMock;
    private readonly AuthorizationApiKeyService<TestUser, ApiKey> _service;

    public AuthorizationApiKeyServiceTest()
    {
        _apiKeyRepositoryMock = new Mock<IRepository<ApiKey>>();
        _fakeUserRepositoryMock = new Mock<IRepository<TestUser>>();
        var httpContextServiceMock = new Mock<IHttpContextService>();

        var optionsMock = new Mock<IOptions<ApiKeyAuthenticationOptions>>();
        optionsMock.Setup(o => o.Value).Returns(new ApiKeyAuthenticationOptions());

        _service = new AuthorizationApiKeyService<TestUser, ApiKey>(
            _apiKeyRepositoryMock.Object,
            _fakeUserRepositoryMock.Object,
            httpContextServiceMock.Object,
            optionsMock.Object
        );
    }

    [Fact]
    public void AuthenticateApiUser_ShouldReturnError_WhenApiKeyIsNullOrEmpty()
    {
        var result = _service.AuthorizeApiUser(null);
        Assert.True(result.HasError);
        Assert.Equal(new AuthorizationTokenNotFoundException().GetReference(), result.Errors.First().Reference);
    }

    [Fact]
    public void AuthenticateApiUser_ShouldReturnError_WhenApiKeyDoesNotExist()
    {
        _apiKeyRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<ApiKey, bool>>>()))
            .Returns(Enumerable.Empty<ApiKey>().AsQueryable());

        var result = _service.AuthorizeApiUser(new ApiKey().Key);
        Assert.True(result.HasError);
        Assert.Equal(new AuthorizationInvalidTokenException().GetReference(), result.Errors.First().Reference);
    }

    [Fact]
    public void AuthenticateApiUser_ShouldReturnError_WhenApiKeyIsExpired()
    {
        var expiredApiKey = new ApiKey { ApiUserId = Guid.Empty, ExpiredAt = DateTime.UtcNow.AddHours(-1) };
        _apiKeyRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<ApiKey, bool>>>()))
            .Returns(new[] { expiredApiKey }.AsQueryable());

        var result = _service.AuthorizeApiUser(expiredApiKey.Key);
        Assert.True(result.HasError);
        Assert.Equal(new AuthorizationExpiredTokenException().GetReference(), result.Errors.First().Reference);
    }

    [Fact]
    public void AuthenticateApiUser_ShouldReturnError_WhenApiUserDoesNotExist()
    {
        var validApiKey = new ApiKey { ApiUserId = Guid.Empty, ExpiredAt = DateTime.UtcNow.AddHours(1) };
        _apiKeyRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<ApiKey, bool>>>()))
            .Returns(new[] { validApiKey }.AsQueryable());

        _fakeUserRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<TestUser, bool>>>()))
            .Returns(Enumerable.Empty<TestUser>().AsQueryable());

        var result = _service.AuthorizeApiUser(validApiKey.Key);
        Assert.True(result.HasError);
        Assert.Equal(new AuthorizationInvalidTokenException().GetReference(), result.Errors.First().Reference);
    }

    [Fact]
    public void AuthenticateApiUser_ShouldReturnSuccess_WhenApiKeyIsValid()
    {
        var validApiKey = new ApiKey { ApiUserId = Guid.Empty, ExpiredAt = DateTime.UtcNow.AddHours(1) };
        _apiKeyRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<ApiKey, bool>>>()))
            .Returns(new[] { validApiKey }.AsQueryable());

        _fakeUserRepositoryMock
            .Setup(r => r.Get(It.IsAny<Expression<Func<TestUser, bool>>>()))
            .Returns(new[] { new TestUser() }.AsQueryable());

        var result = _service.AuthorizeApiUser(validApiKey.Key);
        Assert.False(result.HasError);
    }
}