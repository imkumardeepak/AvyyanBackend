using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class ChatMessage : BaseEntity
    {
        public int ChatRoomId { get; set; }
        public ChatRoom ChatRoom { get; set; } = null!;
        
        public int SenderId { get; set; }
        public User Sender { get; set; } = null!;
        
        [Required]
        [MaxLength(5000)]
        public string Content { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string MessageType { get; set; } = "Text"; // Text, Image, File, System
        
        [MaxLength(500)]
        public string? FileUrl { get; set; }
        
        [MaxLength(200)]
        public string? FileName { get; set; }
        
        [MaxLength(50)]
        public string? FileType { get; set; }
        
        public long? FileSize { get; set; }
        
        public int? ReplyToMessageId { get; set; }
        public ChatMessage? ReplyToMessage { get; set; }
        
        public bool IsEdited { get; set; } = false;
        
        public DateTime? EditedAt { get; set; }
        
        public bool IsDeleted { get; set; } = false;
        
        public DateTime? DeletedAt { get; set; }
        
        // Navigation Properties
        public ICollection<ChatMessage> Replies { get; set; } = new List<ChatMessage>();
        public ICollection<MessageReaction> Reactions { get; set; } = new List<MessageReaction>();
    }
}
