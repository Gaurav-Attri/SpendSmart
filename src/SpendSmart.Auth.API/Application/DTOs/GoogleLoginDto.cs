using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Auth.Application.DTOs;

public class GoogleLoginDto
{
    [Required]
    public string IdToken { get; set; } = string.Empty;
    
    public string? AccessToken { get; set; }
}