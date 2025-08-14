namespace AvyyanBackend.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? ActionUrl { get; set; }
        public string? ActionText { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public string? Metadata { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
    }

    public class CreateNotificationDto
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "Info";
        public string? Category { get; set; }
        public string? ActionUrl { get; set; }
        public string? ActionText { get; set; }
        public bool IsPush { get; set; } = true;
        public bool IsEmail { get; set; } = false;
        public bool IsSms { get; set; } = false;
        public DateTime? ScheduledAt { get; set; }
        public string? Metadata { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
    }

    public class BulkNotificationDto
    {
        public List<int> UserIds { get; set; } = new();
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "Info";
        public string? Category { get; set; }
        public string? ActionUrl { get; set; }
        public string? ActionText { get; set; }
        public bool IsPush { get; set; } = true;
        public bool IsEmail { get; set; } = false;
        public bool IsSms { get; set; } = false;
        public DateTime? ScheduledAt { get; set; }
        public string? Metadata { get; set; }
    }

    public class NotificationSettingsDto
    {
        public int UserId { get; set; }
        public bool EnablePushNotifications { get; set; } = true;
        public bool EnableEmailNotifications { get; set; } = true;
        public bool EnableSmsNotifications { get; set; } = false;
        public bool EnableChatNotifications { get; set; } = true;
        public bool EnableOrderNotifications { get; set; } = true;
        public bool EnableSystemNotifications { get; set; } = true;
        public string? QuietHoursStart { get; set; } // "22:00"
        public string? QuietHoursEnd { get; set; } // "08:00"
        public List<string> MutedCategories { get; set; } = new();
    }

    public class NotificationStatsDto
    {
        public int TotalNotifications { get; set; }
        public int UnreadNotifications { get; set; }
        public int TodayNotifications { get; set; }
        public int WeekNotifications { get; set; }
        public Dictionary<string, int> NotificationsByType { get; set; } = new();
        public Dictionary<string, int> NotificationsByCategory { get; set; } = new();
    }
}
