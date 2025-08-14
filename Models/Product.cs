using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class Product : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string SKU { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CostPrice { get; set; }
        
        public int StockQuantity { get; set; }
        
        public int MinStockLevel { get; set; }
        
        [MaxLength(50)]
        public string? Color { get; set; }
        
        [MaxLength(20)]
        public string? Size { get; set; }
        
        [MaxLength(100)]
        public string? Material { get; set; }
        
        [MaxLength(50)]
        public string? Brand { get; set; }
        
        [MaxLength(20)]
        public string? Weight { get; set; }
        
        [MaxLength(100)]
        public string? Dimensions { get; set; }
        
        public bool IsFeatured { get; set; }
        
        public DateTime? LaunchDate { get; set; }
        
        // Foreign Keys
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        
        // Navigation Properties
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
