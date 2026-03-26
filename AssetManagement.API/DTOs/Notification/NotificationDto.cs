using AssetManagement.API.Models;

namespace AssetManagement.API.DTOs.Notification;

public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? Type { get; set; }
    public bool IsRead { get; set; }
    public Guid? ReferenceId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
