using Digital.Net.Authentication.Attributes;
using InternalTestProgram.Models;
using Microsoft.AspNetCore.Mvc;

namespace InternalTestProgram.Controllers;

[ApiController]
public class TestAuthorizeController : ControllerBase
{
    public const string TestApiKeyRoute = "test/api-key";
    public const string TestApiKeyOrJwtRoute = "test/api-key/jwt";
    public const string TestFakeUserApiKeyRoute = "test/fake-user/api-key";

    [HttpGet(TestApiKeyRoute), Authorize<TestUser>(AuthorizeType.ApiKey)]
    public IActionResult TestApiKey() => Ok();

    [HttpGet(TestFakeUserApiKeyRoute), Authorize<FakeUser>(AuthorizeType.ApiKey)]
    public IActionResult TestFakeUserApiKey() => Ok();

    [HttpGet(TestApiKeyOrJwtRoute), Authorize<TestUser>(AuthorizeType.ApiKey | AuthorizeType.Jwt)]
    public IActionResult TestApiKeyOrJwt() => Ok();
}