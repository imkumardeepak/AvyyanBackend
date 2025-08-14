using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class Category : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [MaxLength(50)]
        public string? Code { get; set; }
        
        public int? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
