using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using AvyyanBackend.Interfaces;
using System.Security.Claims;

namespace AvyyanBackend.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(INotificationService notificationService, ILogger<NotificationHub> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                _logger.LogInformation("User {UserId} connected to notifications with connection {ConnectionId}", userId, Context.ConnectionId);
                
                // Add user to their personal notification group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId.Value}");
                
                // Send unread notification count
                var unreadCount = await _notificationService.GetUnreadNotificationCountAsync(userId.Value);
                await Clients.Caller.SendAsync("UnreadNotificationCount", unreadCount);
                
                // Send recent unread notifications
                var recentNotifications = await _notificationService.GetRecentNotificationsAsync(userId.Value, 10);
                await Clients.Caller.SendAsync("RecentNotifications", recentNotifications);
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                _logger.LogInformation("User {UserId} disconnected from notifications", userId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId.Value}");
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        public async Task MarkNotificationAsRead(int notificationId)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                {
                    await Clients.Caller.SendAsync("Error", "User not authenticated");
                    return;
                }

                await _notificationService.MarkAsReadAsync(userId.Value, notificationId);
                
                // Send updated unread count
                var unreadCount = await _notificationService.GetUnreadNotificationCountAsync(userId.Value);
                await Clients.Caller.SendAsync("UnreadNotificationCount", unreadCount);
                
                _logger.LogDebug("Notification {NotificationId} marked as read by user {UserId}", notificationId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                await Clients.Caller.SendAsync("Error", "Failed to mark notification as read");
            }
        }

        public async Task MarkAllNotificationsAsRead()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                {
                    await Clients.Caller.SendAsync("Error", "User not authenticated");
                    return;
                }

                await _notificationService.MarkAllAsReadAsync(userId.Value);
                
                // Send updated unread count (should be 0)
                await Clients.Caller.SendAsync("UnreadNotificationCount", 0);
                
                _logger.LogDebug("All notifications marked as read by user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                await Clients.Caller.SendAsync("Error", "Failed to mark all notifications as read");
            }
        }

        public async Task GetNotifications(int page = 1, int pageSize = 20)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                {
                    await Clients.Caller.SendAsync("Error", "User not authenticated");
                    return;
                }

                var notifications = await _notificationService.GetNotificationsAsync(userId.Value, page, pageSize);
                await Clients.Caller.SendAsync("NotificationsList", notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications");
                await Clients.Caller.SendAsync("Error", "Failed to get notifications");
            }
        }

        public async Task DeleteNotification(int notificationId)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                {
                    await Clients.Caller.SendAsync("Error", "User not authenticated");
                    return;
                }

                await _notificationService.DeleteNotificationAsync(userId.Value, notificationId);
                
                // Send updated unread count
                var unreadCount = await _notificationService.GetUnreadNotificationCountAsync(userId.Value);
                await Clients.Caller.SendAsync("UnreadNotificationCount", unreadCount);
                
                await Clients.Caller.SendAsync("NotificationDeleted", notificationId);
                
                _logger.LogDebug("Notification {NotificationId} deleted by user {UserId}", notificationId, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification");
                await Clients.Caller.SendAsync("Error", "Failed to delete notification");
            }
        }

        public async Task JoinAdminNotifications()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue) return;

                // Check if user is admin (you might want to implement proper role checking)
                var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
                if (userRole == "Admin" || userRole == "Manager")
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "AdminNotifications");
                    _logger.LogDebug("User {UserId} joined admin notifications", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining admin notifications");
            }
        }

        private int? GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
