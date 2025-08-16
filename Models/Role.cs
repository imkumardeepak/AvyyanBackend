using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class Role : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        public bool IsSystemRole { get; set; } = false;

        // Navigation Properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RolePageAccess> RolePageAccesses { get; set; } = new List<RolePageAccess>();
    }
}
