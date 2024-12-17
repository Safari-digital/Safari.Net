using Digital.Net.Authentication.Controllers;
using Digital.Net.Authentication.Services.Authentication;
using InternalTestProgram.Models;
using Microsoft.AspNetCore.Mvc;

namespace InternalTestProgram.Controllers;

[ApiController, Route("authentication/testuser")]
public class TestAuthenticationController(
    IAuthenticationService<TestUser> authenticationService
) : AuthenticationController<TestUser>(authenticationService);