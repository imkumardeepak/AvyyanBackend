using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.Models
{
    public class ChatRoomMember : BaseEntity
    {
        public int ChatRoomId { get; set; }
        public ChatRoom ChatRoom { get; set; } = null!;
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Member"; // Admin, Moderator, Member
        
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastReadAt { get; set; }
        
        public bool IsMuted { get; set; } = false;
        
        public bool CanSendMessages { get; set; } = true;
        
        public bool CanInviteMembers { get; set; } = false;
    }
}
