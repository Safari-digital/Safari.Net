using Digital.Net.Authentication.Attributes;
using Digital.Net.Authentication.Controllers.Models;
using Digital.Net.Authentication.Models;
using Digital.Net.Authentication.Services.Authentication;
using Digital.Net.Core.Messages;
using Digital.Net.Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace Digital.Net.Authentication.Controllers;

public abstract class AuthenticationController<TApiUser>(
    IAuthenticationService<TApiUser> authenticationService
) : ControllerBase
    where TApiUser : EntityGuid, IApiUser
{
    [HttpPost("login")]
    public virtual async Task<ActionResult<Result<string>>> Login([FromBody] LoginPayload request)
    {
        var result = await authenticationService.Login(request.Login, request.Password);
        return result.HasError || result.Value is null ? Unauthorized() : Ok(result);
    }

    [HttpPost("refresh")]
    public virtual ActionResult<Result<string>> RefreshTokens()
    {
        var result = authenticationService.RefreshTokens();
        return result.HasError ? Unauthorized() : Ok(result);
    }

    [HttpPost("logout")]
    public virtual async Task<IActionResult> Logout()
    {
        await authenticationService.Logout();
        return NoContent();
    }

    [HttpPost("logout-all")]
    public virtual async Task<IActionResult> LogoutAll()
    {
        await authenticationService.LogoutAll();
        return NoContent();
    }
}