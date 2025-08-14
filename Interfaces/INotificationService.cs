using AvyyanBackend.DTOs;

namespace AvyyanBackend.Interfaces
{
    public interface INotificationService
    {
        // Create Notifications
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createNotificationDto);
        Task<IEnumerable<NotificationDto>> CreateBulkNotificationAsync(BulkNotificationDto bulkNotificationDto);
        Task<NotificationDto> CreateSystemNotificationAsync(string title, string message, string? actionUrl = null);
        
        // Get Notifications
        Task<NotificationDto?> GetNotificationAsync(int notificationId);
        Task<IEnumerable<NotificationDto>> GetNotificationsAsync(int userId, int page = 1, int pageSize = 20);
        Task<IEnumerable<NotificationDto>> GetRecentNotificationsAsync(int userId, int count = 10);
        Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId);
        
        // Notification Actions
        Task<bool> MarkAsReadAsync(int userId, int notificationId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteNotificationAsync(int userId, int notificationId);
        Task<bool> DeleteAllNotificationsAsync(int userId);
        
        // Notification Stats
        Task<int> GetUnreadNotificationCountAsync(int userId);
        Task<NotificationStatsDto> GetNotificationStatsAsync(int userId);
        
        // Notification Settings
        Task<NotificationSettingsDto> GetNotificationSettingsAsync(int userId);
        Task<NotificationSettingsDto> UpdateNotificationSettingsAsync(int userId, NotificationSettingsDto settings);
        
        // Send Notifications (Real-time)
        Task SendNotificationToUserAsync(int userId, NotificationDto notification);
        Task SendNotificationToUsersAsync(IEnumerable<int> userIds, NotificationDto notification);
        Task SendNotificationToAllUsersAsync(NotificationDto notification);
        Task SendNotificationToRoleAsync(string role, NotificationDto notification);
        
        // Business-specific Notifications
        Task SendOrderNotificationAsync(int userId, int orderId, string type, string message);
        Task SendProductNotificationAsync(int userId, int productId, string type, string message);
        Task SendChatNotificationAsync(int userId, int chatRoomId, string senderName, string message);
        Task SendSystemMaintenanceNotificationAsync(DateTime scheduledTime, string message);
        
        // Scheduled Notifications
        Task<IEnumerable<NotificationDto>> GetScheduledNotificationsAsync();
        Task ProcessScheduledNotificationsAsync();
        
        // Admin Functions
        Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync(int page = 1, int pageSize = 50);
        Task<NotificationStatsDto> GetGlobalNotificationStatsAsync();
        Task<bool> DeleteNotificationByIdAsync(int notificationId);
    }
}
