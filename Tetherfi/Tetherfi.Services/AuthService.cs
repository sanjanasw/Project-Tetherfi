using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Tetherfi.Data.Enums;
using Tetherfi.Data.Models;
using Tetherfi.Helpers.Exeptions;
using Tetherfi.Helpers.JWT;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Tetherfi.Data;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tetherfi.Common.Models;
using System.Security.Cryptography;
using Tetherfi.Service.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Tetherfi.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;
    private readonly JWTConfigurations _jwtConfigurations;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, IMapper mapper, ILogger<AuthService> logger,
        IOptions<JWTConfigurations> jwtConfigurations, IHttpContextAccessor httpContextAccessor) =>
        (_context, _userManager, _signInManager, _mapper, _logger, _jwtConfigurations, _httpContextAccessor) =
        (context, userManager, signInManager, mapper, logger, jwtConfigurations.Value, httpContextAccessor);



    public async Task<JWTResult> SignInAsync(string username, string password, string ipAddress = null)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(username) ?? throw new HumanErrorException(HttpStatusCode.Unauthorized, "User not found");
            if (user.Status == RecordStatus.Deleted)
            {
                throw new HumanErrorException(HttpStatusCode.Forbidden, "Cannot login to deleted accounts");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (signInResult is null || !signInResult.Succeeded)
                throw new HumanErrorException(HttpStatusCode.Unauthorized, "Username or password incorrect");

            var userRoles = await _userManager.GetRolesAsync(user);

            var User1 = _mapper.Map<User>(new ApplicationUserData { ApplicationUser = user, Roles = userRoles });

            foreach (var userRole in userRoles)
            {
                await _userManager.AddClaimAsync(user,
                    new Claim(ClaimTypes.Role, userRole));
            }
            var token = GenerateJWT(user, userRoles);

            var jwtResult = new JWTResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                User = _mapper.Map<User>(new ApplicationUserData { ApplicationUser = user, Roles = userRoles })
            };

            if (ipAddress != null)
            {
                var tokens = new List<RefreshToken>();
                var refreshToken = GenerateRefreshToken(ipAddress);
                tokens.Add(refreshToken);
                user.RefreshTokens = tokens;
                _context.Set<ApplicationUser>().Update(user);
                await _context.SaveChangesAsync();
                jwtResult.RefreshToken = refreshToken.Token;
            }

            _logger.LogInformation(string.Format("{0} logged in to the system", username));
            return jwtResult;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<JWTResult> RefreshTokenAsync(string token, string ipAddress)
    {
        try
        {
            var user = _context.Set<ApplicationUser>().Include(x => x.RefreshTokens)
                .SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return null;

            var refreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == token);
            if (!refreshToken.IsActive)
                return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            _context.Update(user);
            await _context.SaveChangesAsync();

            var userRoles = await _userManager.GetRolesAsync(user);

            // generate new jwt
            var jwtToken = GenerateJWT(user, userRoles);
            var jwtResult = new JWTResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                Expiration = jwtToken.ValidTo,
                RefreshToken = newRefreshToken.Token,
                User = _mapper.Map<User>(new ApplicationUserData { ApplicationUser = user, Roles = userRoles })
            };

            return jwtResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }


    public bool RevokeToken(string token, string ipAddress)
    {
        try
        {
            var user = _context.Set<ApplicationUser>().Include(x => x.RefreshTokens)
            .SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            // return false if no user found with token
            if (user is null)
                return false;

            var refreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive)
                return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(user);
            _context.SaveChanges();

            _logger.LogWarning(string.Format("{0}'s refrsh token revoked by {1}", user.UserName, ipAddress));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(GetLoggedInUserId()) ?? throw new HumanErrorException(HttpStatusCode.NotFound, "User not found");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation(string.Format("{0} password changed successfully", user.UserName));
                return true;
            }

            _logger.LogWarning(string.Format("{0} password chanege attempt unsuccessful", user.UserName));
            throw new HumanErrorException(HttpStatusCode.BadRequest, result.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            throw;
        }
    }

    public string GetLoggedInUserId()
    {
        return _httpContextAccessor.HttpContext.User.Claims
            .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
    }

    public string GetCurrentLoggedInUsername()
    {
        return _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)
            .Value;
    }

    private JwtSecurityToken GenerateJWT(ApplicationUser user, IList<string>? userRoles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.GivenName, user.UserName)
        };

        if (userRoles != null)
        {
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfigurations?.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        return new JwtSecurityToken(
            _jwtConfigurations.Issuer,
            _jwtConfigurations.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfigurations.Expires),
            signingCredentials: credentials);
    }

    private RefreshToken GenerateRefreshToken(string ipAddress)
    {
        using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
        var randomBytes = new byte[64];
        rngCryptoServiceProvider.GetBytes(randomBytes);
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            Expires = DateTime.UtcNow.AddDays(7),
            CreatedByIp = ipAddress
        };
    }
}

