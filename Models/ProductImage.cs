using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class ProductImage : BaseEntity
    {
        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string? AltText { get; set; }
        
        public bool IsPrimary { get; set; }
        
        public int SortOrder { get; set; }
        
        [MaxLength(50)]
        public string? ImageType { get; set; } // Main, Thumbnail, Gallery, etc.
        
        // Foreign Key
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
