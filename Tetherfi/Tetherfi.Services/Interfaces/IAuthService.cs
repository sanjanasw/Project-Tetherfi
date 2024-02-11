using Tetherfi.Common.Models;
using Tetherfi.Helpers.JWT;

namespace Tetherfi.Service.Interfaces;

public interface IAuthService
{
    Task<JWTResult> SignInAsync(string username, string password, string ipAddress = null);
    Task<JWTResult> RefreshTokenAsync(string token, string ipAddress);
    bool RevokeToken(string token, string ipAddress);
    Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
    public string GetCurrentLoggedInUsername();
    public string GetLoggedInUserId();
}