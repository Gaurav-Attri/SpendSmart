namespace SpendSmart.Auth.Domain.Entities;

public class User
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Currency { get; set; } = "INR";
    public string? AvatarUrl { get; set; }
    public string Role { get; set; } = "User";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    
    // Google OAuth Fields
    public string? GoogleId { get; set; }
    public bool IsGoogleUser { get; set; } = false;
    public string? GoogleAccessToken { get; set; }
    public string? GoogleRefreshToken { get; set; }
    public DateTime? GoogleTokenExpiry { get; set; }
}