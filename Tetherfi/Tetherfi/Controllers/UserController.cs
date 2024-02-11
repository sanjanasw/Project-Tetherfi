using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tetherfi.Common.Models;
using Tetherfi.Service.Interfaces;

namespace Tetherfi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService) =>
        (_userService) = (userService);

    /// <summary>
    /// Register new user
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/user
    ///     {
    ///         "username": "sanjana",
    ///         "firstName": "sanjana",
    ///         "lastName": "witharanage",
    ///         "email": "sanajnasw99@gmai.com",
    ///         "profilePicture": null,
    ///         "role": 0,
    ///         "password": "$Sanjana1"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Returns updated user data</response>
    /// <response code="400">Return errors</response>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> CreateAsync(Register model)
    {
        return Ok(await _userService.CreateAsync(model));
    }

    /// <summary>
    /// Update user data.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/user/4c7a5bca-395c-45fd-aec4-33064b9af928
    ///     {
    ///         "username": "sanjana",
    ///         "firstName": "sanjana",
    ///         "lastName": "witharanage",
    ///         "email": "sanajnasw99@gmai.com",
    ///         "profilePicture": null,
    ///         "role": 0,
    ///         "password": "$Sanjana1"
    ///     }
    ///
    /// </remarks>
    /// <response code="200">Returns updated user data</response>
    /// <response code="400">Return errors</response>
    /// <response code="404">User not found</response>
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser([FromRoute] string id, UserUpdate model)
    {
        return Ok(await _userService.UpdateUserAsync(id, model));
    }
}
