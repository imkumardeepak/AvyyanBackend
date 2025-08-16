using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class RolePageAccess : BaseEntity
    {
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public int PageAccessId { get; set; }
        public PageAccess PageAccess { get; set; } = null!;

        public bool CanView { get; set; } = true;
        public bool CanCreate { get; set; } = false;
        public bool CanEdit { get; set; } = false;
        public bool CanDelete { get; set; } = false;
        public bool CanExport { get; set; } = false;

        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

        public int? GrantedByUserId { get; set; }
        public User? GrantedByUser { get; set; }
    }
}
