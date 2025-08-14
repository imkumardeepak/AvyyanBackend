using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class Customer : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        [MaxLength(10)]
        public string? Gender { get; set; }
        
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        
        public bool IsEmailVerified { get; set; }
        
        public bool IsPhoneVerified { get; set; }
        
        [MaxLength(50)]
        public string? CustomerType { get; set; } // Retail, Wholesale, etc.
        
        // Navigation Properties
        public ICollection<Address> Addresses { get; set; } = new List<Address>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        
        // Computed Property
        public string FullName => $"{FirstName} {LastName}";
    }
}
