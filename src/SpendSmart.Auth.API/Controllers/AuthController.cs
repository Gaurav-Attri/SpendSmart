using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Auth.Application.DTOs;
using SpendSmart.Auth.Application.Services;

namespace SpendSmart.Auth.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _svc;
    private readonly IConfiguration _config;

    public AuthController(IUserService svc, IConfiguration config)
    {
        _svc = svc;
        _config = config;
    }


    [HttpPost("register")]
    [ProducesResponseType(typeof(object), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var (user, token, refreshToken) = await _svc.RegisterAsync(dto);
            return StatusCode(201, new { user, token, refreshToken });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var (user, token, refreshToken) = await _svc.LoginAsync(dto);
            return Ok(new { user, token, refreshToken });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpGet("google-url")]
    public async Task<IActionResult> GetGoogleAuthUrl()
    {
        var redirectUri = _config["Google:RedirectUri"] ?? "http://localhost:5001/api/auth/google-callback";
        var url = await _svc.GetGoogleAuthUrlAsync(redirectUri);
        return Ok(new { url });
    }

    [HttpGet("google-login")]
    public async Task<IActionResult> GoogleLogin()
    {
        var redirectUri = _config["Google:RedirectUri"] ?? "http://localhost:5001/api/auth/google-callback";
        var url = await _svc.GetGoogleAuthUrlAsync(redirectUri);
        return Redirect(url);
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string? error)
    {
        if (!string.IsNullOrEmpty(error))
            return BadRequest(new { error = $"Google authentication failed: {error}" });

        if (string.IsNullOrEmpty(code))
            return BadRequest(new { error = "Authorization code is missing" });

        try
        {
            var redirectUri = _config["Google:RedirectUri"] ?? "http://localhost:5001/api/auth/google-callback";
            var (user, token, refreshToken) = await _svc.GoogleAuthCodeFlowAsync(code, redirectUri);
            return Ok(new { user, token, refreshToken, provider = "google" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Internal error: {ex.Message}" });
        }
    }

    [HttpPost("google-token")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GoogleTokenLogin([FromBody] GoogleLoginDto dto)
    {
        if (string.IsNullOrEmpty(dto.IdToken))
            return BadRequest(new { error = "IdToken is required" });

        try
        {
            var (user, token, refreshToken) = await _svc.GoogleLoginAsync(dto.IdToken);
            return Ok(new { user, token, refreshToken, provider = "google" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }


    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        try
        {
            var newToken = await _svc.RefreshTokenAsync(dto.RefreshToken);
            return Ok(new { token = newToken });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpPost("revoke-google")]
    [Authorize]
    public async Task<IActionResult> RevokeGoogleAccess()
    {
        var userId = GetCurrentUserId();
        await _svc.RevokeGoogleAccessAsync(userId);
        return Ok(new { message = "Google access revoked successfully" });
    }


    [HttpGet("profile/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetProfile(int userId)
    {
        var user = await _svc.GetUserByIdAsync(userId);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPut("currency/{userId}")]
    [Authorize]
    public async Task<IActionResult> UpdateCurrency(int userId, [FromBody] string currency)
    {
        await _svc.UpdateCurrencyAsync(userId, currency);
        return NoContent();
    }

    [HttpPut("change-password/{userId}")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(int userId, [FromBody] ChangePasswordDto dto)
    {
        try
        {
            await _svc.ChangePasswordAsync(userId, dto);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("profile/{userId}")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UpdateProfileDto dto)
    {
        await _svc.UpdateProfileAsync(userId, dto);
        return NoContent();
    }


    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User not authenticated");
        return int.Parse(userIdClaim.Value);
    }
}