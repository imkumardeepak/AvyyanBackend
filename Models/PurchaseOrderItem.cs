using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class PurchaseOrderItem : BaseEntity
    {
        public int Quantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        
        public int? ReceivedQuantity { get; set; }
        
        public DateTime? ReceivedDate { get; set; }
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        // Foreign Keys
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; } = null!;
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
