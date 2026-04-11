using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Auth.Application.Services;

namespace SpendSmart.Auth.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IUserService _userSvc;

    public AdminController(IUserService userSvc)
    {
        _userSvc = userSvc;
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userSvc.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpDelete("deactivate/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateUser(int userId)
    {
        await _userSvc.DeactivateAccountAsync(userId);
        return NoContent();
    }

    [HttpGet("users/ids")]
    [AllowAnonymous]  // ← ONLY THIS LINE CHANGED
    public async Task<IActionResult> GetAllActiveUserIds()
    {
        var users = await _userSvc.GetAllUsersAsync();
        var ids = users.Where(u => u.IsActive).Select(u => u.UserId).ToList();
        return Ok(ids);
    }
}