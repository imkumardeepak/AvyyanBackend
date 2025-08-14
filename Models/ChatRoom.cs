using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class ChatRoom : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "Group"; // Personal, Group, Channel
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        public bool IsPrivate { get; set; } = false;
        
        public int MaxMembers { get; set; } = 100;
        
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;
        
        // Navigation Properties
        public ICollection<ChatRoomMember> Members { get; set; } = new List<ChatRoomMember>();
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
