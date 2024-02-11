using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tetherfi.Common.Models;
using Tetherfi.Helpers.Exeptions;
using Tetherfi.Service.Interfaces;

namespace Tetherfi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) =>
        (_authService) = (authService);

    /// <summary>
    /// Login to the system
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/login
    ///     {
    ///        "userName": "sanjana",
    ///        "password": "$Sanjana1"
    ///        "rememberMe": true
    ///     }
    ///     
    ///     public route that accepts HTTP POST requests containing a username and password in the body.
    ///     If the username and password are correct then a JWT authentication token and the user details are returned
    ///     in the response body, and a refresh token cookie (HTTP Only) is returned in the response headers.
    /// </remarks>
    /// <param name="model">Email and password are required</param>
    /// <response code="200">Returns user data with JWT</response>
    /// <response code="401">Unothorized user</response>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(Login model)
    {
        var jwtResult = await _authService.SignInAsync(model.Username, model.Password,
            model.RememberMe ? IpAddress() : null);

        if (jwtResult != null)
            return Ok(jwtResult);

        throw new HumanErrorException(HttpStatusCode.Unauthorized, "Incorect username or password");
    }

    /// <summary>
    /// Refresh token
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/v1.0/Accounts/refresh-token
    ///     {
    ///        "token": "jkbvjdgj-crgmhcrhngujrchgjhrckjgrg/kg"
    ///     }
    ///     
    ///     public route that accepts HTTP POST requests with a refresh token cookie.
    ///     If the cookie exists and the refresh token is valid then a new JWT authentication token and the user details are
    ///     returned in the response body, a new refresh token cookie (HTTP Only) is returned in the response headers and the old
    ///     refresh token is revoked.
    /// </remarks>
    /// <param name="model"></param>
    /// <response code="200">Returns user data with JWT</response>
    [Authorize]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRefresh model)
    {
        var response = await _authService.RefreshTokenAsync(model.Token, IpAddress());

        if (response == null)
            return Unauthorized(new { message = "Invalid token" });

        return Ok(response);
    }

    /// <summary>
    /// Revoke refresh token
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/revoke-token
    ///     {
    ///        "token": "iugniuguxyfeg08y8y4nuxenhf-x9uef-xnisej"
    ///     }
    ///     
    ///     Secure route that accepts HTTP POST requests containing a refresh token either in the body or in a cookie,
    ///     if both are present the token in the body is used.
    ///     If the refresh token is valid and active then it is revoked and can no longer be used to refresh JWT tokens.
    /// </remarks>
    /// <param name="model"></param>
    /// <response code="200">Returns user data with JWT</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("revoke-token")]
    public IActionResult RevokeToken(RevokeToken model)
    {
        if (string.IsNullOrEmpty(model.Token))
            return BadRequest(new { message = "Token is required" });

        var response = _authService.RevokeToken(model.Token, IpAddress());

        if (!response)
            return NotFound(new { message = "Token not found" });

        return Ok(new { message = "Token revoked" });
    }


    /// <summary>
    /// Change password
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/auth/change-password
    ///     {
    ///         "currentPassword": "Not@1234",
    ///         "newPassword": "1234@Not"
    ///     }
    ///
    /// </remarks>
    /// <param name="model"></param>
    /// <response code="200">Returns success message</response>
    /// <response code="400">Password doesn't meet minimum requirements</response>
    /// <response code="403">Current password incorrect</response>
    /// <response code="404">User not found</response>
    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePasswrod(ChangePassword model)
    {
        var result = await _authService.ChangePasswordAsync(model.CurrentPassword, model.NewPassword);

        if (result)
        {
            return Ok("Password changed successfully");
        }
        throw new Exception("Password changing unsuccessful");
    }


    private string IpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }
}
