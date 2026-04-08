using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Auth.Application.DTOs;

public class UpdateProfileDto
{
    [StringLength(100, MinimumLength = 2)]
    public string? FullName { get; set; }
    
    [Url]
    public string? AvatarUrl { get; set; }
}