using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class Address : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Street { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string? Street2 { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(20)]
        public string PostalCode { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? AddressType { get; set; } // Home, Office, Billing, Shipping
        
        public bool IsDefault { get; set; }
        
        // Foreign Key
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        
        // Navigation Properties
        public ICollection<Order> BillingOrders { get; set; } = new List<Order>();
        public ICollection<Order> ShippingOrders { get; set; } = new List<Order>();
    }
}
