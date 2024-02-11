using Microsoft.AspNetCore.Identity;
using System.Net;
using Tetherfi.Data;
using Tetherfi.Data.Models;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Tetherfi.Helpers.Exeptions;
using Tetherfi.Common.Models;
using Tetherfi.Common.Enums;
using Tetherfi.Service.Interfaces;

namespace Tetherfi.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
        IMapper mapper, ILogger<UserService> logger, IAuthService authService) =>
        (_context, _userManager, _roleManager, _mapper, _logger, _authService) = (context, userManager, roleManager, mapper, logger, authService);

    public async Task<User> CreateAsync(Register model)
    {
        var user = _mapper.Map<ApplicationUser>(model);
        return await InsertAsync(user, model.Role, model.Password);
    }

    private async Task<User> InsertAsync(ApplicationUser model, UserRole role, string password)
    {
        try
        {
            var result = await _userManager.CreateAsync(model, password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(UserRole.Admin.ToString()))
                    await _roleManager.CreateAsync(new IdentityRole(UserRole.Admin.ToString()));
                if (!await _roleManager.RoleExistsAsync(UserRole.User.ToString()))
                    await _roleManager.CreateAsync(new IdentityRole(UserRole.User.ToString()));

                switch (role)
                {
                    case UserRole.Admin:
                        if (await _roleManager.RoleExistsAsync(UserRole.Admin.ToString()))
                            await _userManager.AddToRoleAsync(model, UserRole.Admin.ToString());
                        break;
                    default:
                        if (await _roleManager.RoleExistsAsync(UserRole.User.ToString()))
                            await _userManager.AddToRoleAsync(model, UserRole.User.ToString());
                        break;
                }


                _logger.LogInformation(string.Format("{0} new {1} registered successfully.", model.UserName, role.ToString()));
                return _mapper.Map<User>(new ApplicationUserData { ApplicationUser = model, Roles = await _userManager.GetRolesAsync(model) });
            }

            _logger.LogWarning(result.Errors.First().Description);
            throw new HumanErrorException(HttpStatusCode.Conflict, result.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<User> UpdateUserAsync(string userId, UserUpdate model)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId) ?? throw new HumanErrorException(HttpStatusCode.NotFound, "User not found");

            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Dob = model.Dob;
            user.ProfilePicture = model.ProfilePicture;
            user.ModifiedBy = _authService.GetCurrentLoggedInUsername();
            user.ModifiedOn = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return _mapper.Map<User>(new ApplicationUserData
                {
                    ApplicationUser = user,
                    Roles = await _userManager.GetRolesAsync(user),
                });
            }

            throw new HumanErrorException(HttpStatusCode.BadRequest, result.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }
}

