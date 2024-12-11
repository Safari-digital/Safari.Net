using Digital.Net.Authentication.Attributes;
using Digital.Net.Authentication.Models;
using Microsoft.AspNetCore.Mvc;

namespace InternalTestProgram.Controllers;

[ApiController]
public class TestAuthenticationController : ControllerBase
{
    public const string TestApiKeyRoute = "test/api-key";
    public const string TestApiKeyOrJwtRoute = "test/api-key/jwt";

    [HttpGet(TestApiKeyRoute), Authorize(AuthorizeType.ApiKey)]
    public IActionResult TestApiKey() => Ok();

    [HttpGet(TestApiKeyOrJwtRoute), Authorize(AuthorizeType.ApiKey | AuthorizeType.Jwt)]
    public IActionResult TestApiKeyOrJwt() => Ok();
}