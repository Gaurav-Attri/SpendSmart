using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Auth.Application.Services;

namespace SpendSmart.Auth.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IUserService _userSvc;

    public AdminController(IUserService userSvc)
    {
        _userSvc = userSvc;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userSvc.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpDelete("deactivate/{userId}")]
    public async Task<IActionResult> DeactivateUser(int userId)
    {
        await _userSvc.DeactivateAccountAsync(userId);
        return NoContent();
    }

    [HttpGet("users/ids")]
    public async Task<IActionResult> GetAllActiveUserIds()
    {
        var users = await _userSvc.GetAllUsersAsync();
        var ids = users.Where(u => u.IsActive).Select(u => u.UserId).ToList();
        return Ok(ids);
    }
}