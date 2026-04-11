using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Notification.API.Application.DTOs;

public class BroadcastDto
{
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string Message { get; set; } = string.Empty;
}