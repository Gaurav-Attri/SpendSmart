using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Auth.Application.DTOs;

public class RegisterDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", 
        ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number, and one special character")]
    public string Password { get; set; } = string.Empty;

    [StringLength(3, MinimumLength = 3)]
    [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be a 3-letter ISO code (e.g., USD, INR, EUR)")]
    public string Currency { get; set; } = "INR";
}