using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class PurchaseOrder : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string PurchaseOrderNumber { get; set; } = string.Empty;
        
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        
        public DateTime? ExpectedDeliveryDate { get; set; }
        
        public DateTime? ActualDeliveryDate { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Draft"; // Draft, Sent, Confirmed, Received, Cancelled
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        // Foreign Key
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; } = null!;
        
        // Navigation Properties
        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    }
}
