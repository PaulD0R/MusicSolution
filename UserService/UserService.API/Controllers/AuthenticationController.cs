using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.API.Extensions;
using UserService.Application.Interfaces.Services;
using UserService.Application.Models.Person;
using UserService.Application.Models.Token;

namespace UserService.API.Controllers;

[ApiController]
[Route("authentication")]
public class AuthenticationController(
    IAuthenticationService authenticationService,
    IRefreshTokenService refreshTokenService,
    ILogger<AuthenticationController> logger)
    : ControllerBase
{
    [HttpPost("signin")]
    public async Task<IActionResult> Signin([FromBody] SigninRequest signinRequest)
    {   
        logger.LogInformation("TCP GET /signin");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Ok(await authenticationService.SigninAsync(signinRequest));
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupRequest newPersonRequest)
    {
        logger.LogInformation("TCP GET /signup");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Ok(await authenticationService.SignupAsync(newPersonRequest));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        logger.LogInformation("TCP GET /refresh-token");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Ok(await refreshTokenService.RefreshTokenAsync(request));
    }

    [HttpDelete("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        logger.LogInformation("TCP GET /logout");
        var personId = User.GetId();
        if (personId == null) return Unauthorized("Не авторизирован");

        await authenticationService.LogoutAsync(personId);
        return NoContent();
    }
}