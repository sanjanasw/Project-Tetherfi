using System.ComponentModel.DataAnnotations;

namespace Tetherfi.Common.Models;

public class Login
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}

public class RefreshTokenRefresh
{
    [Required(ErrorMessage = "Token is Required")]
    public string Token { get; set; }
}

public class RevokeToken
{
    [Required(ErrorMessage = "Token is Required")]
    public string Token { get; set; }
}

public class ForgetPassword
{
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
}

public class ResetPassword
{
    [Required(ErrorMessage = "User Id is Required")]
    public string Userid { get; set; }

    [Required(ErrorMessage = "Token is Required")]
    public string Token { get; set; }

    [Required(ErrorMessage = "Password is Required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}

public class ChangePassword
{
    [Required(ErrorMessage = "Current Password is Required")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "New Password is Required")]
    [DataType(DataType.Password)]
    public string NewPassword { get; set; }
}

