using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class PageAccess : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string PageName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string PageUrl { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }

        [MaxLength(50)]
        public string? Icon { get; set; }

        public int SortOrder { get; set; } = 0;

        public bool IsMenuItem { get; set; } = true;

        // Navigation Properties
        public ICollection<RolePageAccess> RolePageAccesses { get; set; } = new List<RolePageAccess>();
    }
}
