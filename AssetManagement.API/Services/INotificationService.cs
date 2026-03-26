using AssetManagement.API.Models;

namespace AssetManagement.API.Services;

public interface INotificationService
{
    Task<IEnumerable<DTOs.Notification.NotificationDto>> GetUserNotificationsAsync(Guid userId);
    Task MarkAsReadAsync(Guid id, Guid userId);
    Task MarkAllAsReadAsync(Guid userId);
    Task CreateNotificationAsync(Guid userId, string title, string message, string type, Guid? referenceId = null);
}
