using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class Order : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string OrderNumber { get; set; } = string.Empty;
        
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        [MaxLength(50)]
        public string? PaymentMethod { get; set; }
        
        [MaxLength(50)]
        public string? PaymentStatus { get; set; } // Pending, Paid, Failed, Refunded
        
        public DateTime? PaymentDate { get; set; }
        
        [MaxLength(100)]
        public string? PaymentTransactionId { get; set; }
        
        public DateTime? ShippedDate { get; set; }
        
        public DateTime? DeliveredDate { get; set; }
        
        [MaxLength(100)]
        public string? TrackingNumber { get; set; }
        
        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        // Foreign Keys
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        
        public int? BillingAddressId { get; set; }
        public Address? BillingAddress { get; set; }
        
        public int? ShippingAddressId { get; set; }
        public Address? ShippingAddress { get; set; }
        
        // Navigation Properties
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
