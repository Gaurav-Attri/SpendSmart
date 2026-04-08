using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Auth.Application.DTOs;

public class ChangePasswordDto
{
    [Required]
    public string OldPassword { get; set; } = string.Empty;

    [Required, MinLength(8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number, and one special character")]
    public string NewPassword { get; set; } = string.Empty;
}