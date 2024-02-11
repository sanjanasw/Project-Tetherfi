using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Tetherfi.Data.Enums;

namespace Tetherfi.Data.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [PersonalData]
    public string? FirstName { get; set; }

    [Required]
    [PersonalData]
    public string? LastName { get; set; }

    public string? ProfilePicture { get; set; }

    public DateTime Dob { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? DeletedOn { get; set; }

    public string? DeletedBy { get; set; }

    [Required]
    public RecordStatus Status { get; set; } = RecordStatus.Active;

    [JsonIgnore]
    public virtual List<RefreshToken> RefreshTokens { get; set; }

}