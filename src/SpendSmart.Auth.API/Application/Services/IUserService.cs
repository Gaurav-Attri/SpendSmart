using SpendSmart.Auth.Application.DTOs;

namespace SpendSmart.Auth.Application.Services;

public interface IUserService
{
    Task<(UserResponseDto user, string token, string refreshToken)> RegisterAsync(RegisterDto dto);
    Task<(UserResponseDto user, string token, string refreshToken)> LoginAsync(LoginDto dto);
    Task<UserResponseDto?> GetUserByIdAsync(int userId);
    Task<List<UserResponseDto>> GetAllUsersAsync();
    Task UpdateProfileAsync(int userId, UpdateProfileDto dto);
    Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task UpdateCurrencyAsync(int userId, string currency);
    Task DeactivateAccountAsync(int userId);
    Task<(UserResponseDto user, string token, string refreshToken)> GoogleLoginAsync(string idToken);
    Task<(UserResponseDto user, string token, string refreshToken)> GoogleAuthCodeFlowAsync(string code, string redirectUri);
    Task<string> RefreshTokenAsync(string refreshToken);
    Task RevokeGoogleAccessAsync(int userId);
    Task<string> GetGoogleAuthUrlAsync(string redirectUri);
}