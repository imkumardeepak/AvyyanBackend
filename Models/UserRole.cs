using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class UserRole : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public int? AssignedByUserId { get; set; }
        public User? AssignedByUser { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public new bool IsActive { get; set; } = true;
    }
}
