using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SpendSmart.Auth.Application.DTOs;
using SpendSmart.Auth.Domain.Entities;
using SpendSmart.Auth.Infrastructure.Repositories;
using SpendSmart.Auth.API.Infrastructure.HttpClients;

namespace SpendSmart.Auth.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;
    private readonly ICategoryHttpClient _categoryClient;
    private readonly PasswordHasher<User> _hasher = new();

    public UserService(IUserRepository repo, IConfiguration config, HttpClient httpClient, ICategoryHttpClient categoryClient)
    {
        _repo = repo;
        _config = config;
        _httpClient = httpClient;
        _categoryClient = categoryClient;
    }

    public async Task<(UserResponseDto user, string token, string refreshToken)> RegisterAsync(RegisterDto dto)
    {
        if (await _repo.ExistsByEmailAsync(dto.Email))
            throw new InvalidOperationException("Email already registered.");

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Currency = dto.Currency,
            IsGoogleUser = false
        };

        user.PasswordHash = _hasher.HashPassword(user, dto.Password);
        await _repo.AddAsync(user);
        await _repo.SaveChangesAsync();

        // Seed default categories for the new user
        await _categoryClient.SeedDefaultCategoriesAsync(user.UserId);

        var (token, refreshToken) = GenerateTokens(user);
        return (MapToDto(user), token, refreshToken);
    }

    public async Task<(UserResponseDto user, string token, string refreshToken)> LoginAsync(LoginDto dto)
    {
        var user = await _repo.FindByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account suspended.");

        if (user.IsGoogleUser)
            throw new UnauthorizedAccessException("Please login using Google.");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid credentials.");

        await _repo.UpdateLastLoginAsync(user.UserId, DateTime.UtcNow);
        
        var (token, refreshToken) = GenerateTokens(user);
        return (MapToDto(user), token, refreshToken);
    }
    
    public async Task<(UserResponseDto user, string token, string refreshToken)> GoogleLoginAsync(string idToken)
    {
        var googleUserInfo = await VerifyGoogleIdTokenAsync(idToken);
        
        if (string.IsNullOrEmpty(googleUserInfo.Email))
            throw new UnauthorizedAccessException("Invalid Google token.");

        var user = await _repo.FindByGoogleIdOrEmailAsync(googleUserInfo.Id, googleUserInfo.Email);
        
        if (user == null)
        {
            user = new User
            {
                FullName = googleUserInfo.Name,
                Email = googleUserInfo.Email,
                GoogleId = googleUserInfo.Id,
                IsGoogleUser = true,
                IsActive = true,
                Role = "User",
                Currency = "USD",
                AvatarUrl = googleUserInfo.Picture,
                CreatedAt = DateTime.UtcNow
            };
            await _repo.AddAsync(user);
            await _repo.SaveChangesAsync();

            // Seed default categories for new Google user
            await _categoryClient.SeedDefaultCategoriesAsync(user.UserId);
        }
        else if (string.IsNullOrEmpty(user.GoogleId))
        {
            user.GoogleId = googleUserInfo.Id;
            user.IsGoogleUser = true;
            if (string.IsNullOrEmpty(user.AvatarUrl))
                user.AvatarUrl = googleUserInfo.Picture;
            await _repo.UpdateAsync(user);
        }

        await _repo.UpdateLastLoginAsync(user.UserId, DateTime.UtcNow);
        await _repo.SaveChangesAsync();

        var (token, refreshToken) = GenerateTokens(user);
        return (MapToDto(user), token, refreshToken);
    }

    public async Task<(UserResponseDto user, string token, string refreshToken)> GoogleAuthCodeFlowAsync(string code, string redirectUri)
    {
        var tokenResponse = await ExchangeGoogleCodeForTokensAsync(code, redirectUri);
        var googleUserInfo = await GetGoogleUserInfoAsync(tokenResponse.AccessToken);
        
        var user = await _repo.FindByGoogleIdOrEmailAsync(googleUserInfo.Id, googleUserInfo.Email);
        
        if (user == null)
        {
            user = new User
            {
                FullName = googleUserInfo.Name,
                Email = googleUserInfo.Email,
                GoogleId = googleUserInfo.Id,
                IsGoogleUser = true,
                IsActive = true,
                Role = "User",
                Currency = "USD",
                AvatarUrl = googleUserInfo.Picture,
                GoogleAccessToken = tokenResponse.AccessToken,
                GoogleRefreshToken = tokenResponse.RefreshToken,
                GoogleTokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                CreatedAt = DateTime.UtcNow
            };
            await _repo.AddAsync(user);
            await _repo.SaveChangesAsync();

            // Seed default categories for new Google user
            await _categoryClient.SeedDefaultCategoriesAsync(user.UserId);
        }
        else
        {
            user.GoogleAccessToken = tokenResponse.AccessToken;
            user.GoogleRefreshToken = tokenResponse.RefreshToken ?? user.GoogleRefreshToken;
            user.GoogleTokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            if (string.IsNullOrEmpty(user.AvatarUrl))
                user.AvatarUrl = googleUserInfo.Picture;
            await _repo.UpdateAsync(user);
        }

        await _repo.UpdateLastLoginAsync(user.UserId, DateTime.UtcNow);
        await _repo.SaveChangesAsync();

        var (token, refreshToken) = GenerateTokens(user);
        return (MapToDto(user), token, refreshToken);
    }

    public async Task<string> RefreshTokenAsync(string refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
        
        try
        {
            var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("Invalid refresh token");

            var userId = int.Parse(userIdClaim.Value);
            var user = await _repo.FindByUserIdAsync(userId);
            
            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("User not found or inactive");

            var (newToken, _) = GenerateTokens(user);
            return newToken;
        }
        catch
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }
    }

    public async Task RevokeGoogleAccessAsync(int userId)
    {
        var user = await _repo.FindByUserIdAsync(userId);
        if (user?.GoogleAccessToken != null)
        {
            await RevokeGoogleTokenAsync(user.GoogleAccessToken);
            user.GoogleAccessToken = null;
            user.GoogleRefreshToken = null;
            user.GoogleTokenExpiry = null;
            await _repo.UpdateAsync(user);
            await _repo.SaveChangesAsync();
        }
    }

    public async Task<string> GetGoogleAuthUrlAsync(string redirectUri)
    {
        var clientId = _config["Google:ClientId"];
        var scope = "openid email profile";
        
        return $"https://accounts.google.com/o/oauth2/v2/auth?" +
               $"client_id={clientId}" +
               $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
               $"&response_type=code" +
               $"&scope={Uri.EscapeDataString(scope)}" +
               $"&access_type=offline" +
               $"&prompt=consent";
    }

    private async Task<GoogleUserInfoDto> VerifyGoogleIdTokenAsync(string idToken)
    {
        var response = await _httpClient.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");
        
        if (!response.IsSuccessStatusCode)
            throw new UnauthorizedAccessException("Invalid Google token.");
        
        var content = await response.Content.ReadAsStringAsync();
        var userInfo = JsonSerializer.Deserialize<GoogleUserInfoDto>(content);
        
        if (userInfo == null || string.IsNullOrEmpty(userInfo.Email))
            throw new UnauthorizedAccessException("Invalid Google token.");
        
        return userInfo;
    }

    private async Task<GoogleTokenResponse> ExchangeGoogleCodeForTokensAsync(string code, string redirectUri)
    {
        var clientId = _config["Google:ClientId"];
        var clientSecret = _config["Google:ClientSecret"];
        
        var requestBody = new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = clientId!,
            ["client_secret"] = clientSecret!,
            ["redirect_uri"] = redirectUri,
            ["grant_type"] = "authorization_code"
        };
        
        var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", 
            new FormUrlEncodedContent(requestBody));
        
        if (!response.IsSuccessStatusCode)
            throw new UnauthorizedAccessException("Failed to exchange code for tokens.");
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GoogleTokenResponse>(content) 
            ?? throw new UnauthorizedAccessException("Invalid token response");
    }

    private async Task<GoogleUserInfoDto> GetGoogleUserInfoAsync(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var response = await _httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
        
        if (!response.IsSuccessStatusCode)
            throw new UnauthorizedAccessException("Failed to get user info from Google.");
        
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GoogleUserInfoDto>(content) 
            ?? throw new UnauthorizedAccessException("Invalid user info response");
    }

    private async Task RevokeGoogleTokenAsync(string accessToken)
    {
        await _httpClient.PostAsync($"https://oauth2.googleapis.com/revoke?token={accessToken}", null);
    }

    private (string token, string refreshToken) GenerateTokens(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role),
            new("currency", user.Currency),
            new("isGoogleUser", user.IsGoogleUser.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24),
            Issuer = _config["Jwt:Issuer"],
            Audience = _config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        
        var refreshToken = GenerateRefreshToken();
        
        return (tokenString, refreshToken);
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int userId)
    {
        var u = await _repo.FindByUserIdAsync(userId);
        return u == null ? null : MapToDto(u);
    }

    public async Task<List<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _repo.FindAllActiveAsync();
        return users.Select(MapToDto).ToList();
    }

    public async Task UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _repo.FindByUserIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");
        
        if (!string.IsNullOrWhiteSpace(dto.FullName))
            user.FullName = dto.FullName;
        if (dto.AvatarUrl != null)
            user.AvatarUrl = dto.AvatarUrl;
            
        await _repo.UpdateAsync(user);
        await _repo.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _repo.FindByUserIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");
            
        if (user.IsGoogleUser)
            throw new InvalidOperationException("Google users cannot change password. Use Google login.");
            
        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.OldPassword);
        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Old password is incorrect.");

        user.PasswordHash = _hasher.HashPassword(user, dto.NewPassword);
        await _repo.UpdateAsync(user);
        await _repo.SaveChangesAsync();
    }

    public async Task DeactivateAccountAsync(int userId)
    {
        var user = await _repo.FindByUserIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");
        user.IsActive = false;
        await _repo.UpdateAsync(user);
        await _repo.SaveChangesAsync();
    }

    public Task UpdateCurrencyAsync(int userId, string currency) =>
        _repo.UpdateCurrencyAsync(userId, currency);

    private static UserResponseDto MapToDto(User u) => new()
    {
        UserId = u.UserId,
        FullName = u.FullName,
        Email = u.Email,
        Currency = u.Currency,
        AvatarUrl = u.AvatarUrl,
        Role = u.Role,
        IsActive = u.IsActive,
        CreatedAt = u.CreatedAt,
        IsGoogleUser = u.IsGoogleUser
    };
}

public class GoogleTokenResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;
    
    [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("id_token")]
    public string? IdToken { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}