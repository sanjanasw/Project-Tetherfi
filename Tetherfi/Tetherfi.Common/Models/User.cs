using System.ComponentModel.DataAnnotations;
using Tetherfi.Common.Enums;
using Tetherfi.Data.Models;

namespace Tetherfi.Common.Models;

public class User
{
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string ProfilePicture { get; set; }

    public DateTime Dob { get; set; }

    public IList<string> Roles { get; set; }
}

public class Register
{
    [Required(ErrorMessage = "First Name is Required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is Required")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Username is Required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Email is Required")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required(ErrorMessage = "Role is Required")]
    public UserRole Role { get; set; }

    [Required(ErrorMessage = "Password is Required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Birthday is Required")]
    public DateTime Dob { get; set; }

    public string? ProfilePicture { get; set; }
}

public class UserUpdate
{

    [Required(ErrorMessage = "First Name is Required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is Required")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Username is Required")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Email is Required")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    public string? ProfilePicture { get; set; }

    [Required(ErrorMessage = "Birthday is Required")]
    public DateTime Dob { get; set; }

}

public class ApplicationUserData
{
    public ApplicationUser ApplicationUser { get; set; }

    public IList<string> Roles { get; set; }
}

