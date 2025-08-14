using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "Info"; // Info, Warning, Error, Success, Chat, Order, System
        
        [MaxLength(50)]
        public string? Category { get; set; } // Order, Product, Chat, System, etc.
        
        [MaxLength(500)]
        public string? ActionUrl { get; set; }
        
        [MaxLength(100)]
        public string? ActionText { get; set; }
        
        public bool IsRead { get; set; } = false;
        
        public DateTime? ReadAt { get; set; }
        
        public bool IsPush { get; set; } = true; // Should send push notification
        
        public bool IsEmail { get; set; } = false; // Should send email notification
        
        public bool IsSms { get; set; } = false; // Should send SMS notification
        
        public DateTime? ScheduledAt { get; set; } // For scheduled notifications
        
        public DateTime? SentAt { get; set; }
        
        [MaxLength(1000)]
        public string? Metadata { get; set; } // JSON data for additional context
        
        public int? RelatedEntityId { get; set; } // ID of related entity (order, product, etc.)
        
        [MaxLength(50)]
        public string? RelatedEntityType { get; set; } // Order, Product, User, etc.
    }
}
