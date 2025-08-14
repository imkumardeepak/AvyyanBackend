using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class MessageReaction : BaseEntity
    {
        public int MessageId { get; set; }
        public ChatMessage Message { get; set; } = null!;
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        [MaxLength(10)]
        public string Emoji { get; set; } = string.Empty; // 👍, ❤️, 😂, etc.
    }
}
