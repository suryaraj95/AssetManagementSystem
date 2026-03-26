using AssetManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AssetManagement.API.Endpoints;

public static class NotificationEndpoints
{
    public static void MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notifications").RequireAuthorization();

        group.MapGet("/", async (INotificationService notificationService, ClaimsPrincipal user) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId)) return Results.Unauthorized();

            var notifications = await notificationService.GetUserNotificationsAsync(userId);
            return Results.Ok(notifications);
        });

        group.MapPut("/{id}/read", async (Guid id, INotificationService notificationService, ClaimsPrincipal user) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId)) return Results.Unauthorized();

            await notificationService.MarkAsReadAsync(id, userId);
            return Results.NoContent();
        });

        group.MapPut("/read-all", async (INotificationService notificationService, ClaimsPrincipal user) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId)) return Results.Unauthorized();

            await notificationService.MarkAllAsReadAsync(userId);
            return Results.NoContent();
        });
    }
}
