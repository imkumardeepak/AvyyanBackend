using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;
        
        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
        
        [MaxLength(500)]
        public string? ProfileImageUrl { get; set; }
        
        [MaxLength(50)]
        public string? Role { get; set; } = "User"; // Admin, Manager, Employee, Customer
        
        [MaxLength(50)]
        public string? Department { get; set; }
        
        [MaxLength(100)]
        public string? JobTitle { get; set; }
        
        public bool IsOnline { get; set; }
        
        public DateTime? LastSeenAt { get; set; }
        
        public bool IsEmailVerified { get; set; }
        
        // Navigation Properties
        public ICollection<ChatMessage> SentMessages { get; set; } = new List<ChatMessage>();
        public ICollection<ChatRoomMember> ChatRoomMemberships { get; set; } = new List<ChatRoomMember>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<UserConnection> Connections { get; set; } = new List<UserConnection>();
        
        // Computed Property
        public string FullName => $"{FirstName} {LastName}";
    }
}
