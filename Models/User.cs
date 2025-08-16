using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? ProfilePicture { get; set; }

        public bool IsEmailVerified { get; set; } = false;

        public bool IsLocked { get; set; } = false;

        public int FailedLoginAttempts { get; set; } = 0;

        public DateTime? LastLoginAt { get; set; }

        public DateTime? LockedUntil { get; set; }

        [MaxLength(255)]
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Navigation Properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<ChatRoomMember> ChatRoomMemberships { get; set; } = new List<ChatRoomMember>();
        public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<UserConnection> UserConnections { get; set; } = new List<UserConnection>();

        // Computed Properties
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        public bool IsOnline => UserConnections.Any(uc => uc.IsActive);
    }
}
