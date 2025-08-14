using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class UserConnection : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string ConnectionId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? UserAgent { get; set; }

        [MaxLength(50)]
        public string? IpAddress { get; set; }

        [MaxLength(50)]
        public string? DeviceType { get; set; } // Web, Mobile, Desktop

        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DisconnectedAt { get; set; }

        public new bool IsActive { get; set; } = true;
    }
}
