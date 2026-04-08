using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Auth.Application.DTOs;

public class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}