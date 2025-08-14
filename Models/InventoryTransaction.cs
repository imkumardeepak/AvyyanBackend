using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class InventoryTransaction : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string TransactionType { get; set; } = string.Empty; // In, Out, Adjustment
        
        public int Quantity { get; set; }
        
        public int PreviousStock { get; set; }
        
        public int NewStock { get; set; }
        
        [MaxLength(200)]
        public string? Reason { get; set; }
        
        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
        // Foreign Key
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
