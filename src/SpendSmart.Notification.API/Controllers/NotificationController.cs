using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Notification.API.Application.DTOs;
using SpendSmart.Notification.API.Application.Services;
using SpendSmart.Notification.API.Domain.Entities;

namespace SpendSmart.Notification.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationRecordService _svc;

    public NotificationController(INotificationRecordService svc) => _svc = svc;

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User not authenticated");
        return int.Parse(userIdClaim.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(NotificationRecord), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateNotificationRecordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var notification = await _svc.CreateNotificationAsync(dto);
            return StatusCode(201, notification);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("budget-alert")]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SendBudgetAlert([FromBody] BudgetAlertDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _svc.SendBudgetAlertAsync(dto.UserId, dto.AlertType, dto.Percent, dto.BudgetId);
            return Ok(new { message = "Budget alert sent successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(List<NotificationRecord>), 200)]
    public async Task<IActionResult> GetUserNotifications(
        int userId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var notifications = await _svc.GetUserNotificationsAsync(userId, page, pageSize);
        return Ok(notifications);
    }

    [HttpGet("unread-count/{userId}")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> GetUnreadCount(int userId)
    {
        var count = await _svc.GetUnreadCountAsync(userId);
        return Ok(new { userId, unreadCount = count });
    }

    [HttpPut("{id}/read")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var currentUserId = GetCurrentUserId();
        var notifications = await _svc.GetUserNotificationsAsync(currentUserId, 1, 100);
        var notification = notifications.FirstOrDefault(n => n.NotificationRecordId == id);
        
        if (notification == null)
            return NotFound();

        await _svc.MarkAsReadAsync(id);
        return NoContent();
    }

    [HttpPut("mark-all-read/{userId}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> MarkAllRead(int userId)
    {
        await _svc.MarkAllReadAsync(userId);
        return NoContent();
    }

    [HttpPost("broadcast")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Broadcast([FromBody] BroadcastDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _svc.BroadcastAsync(dto.Title, dto.Message);
            return Ok(new { message = "Broadcast sent successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}