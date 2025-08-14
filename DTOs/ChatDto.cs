namespace AvyyanBackend.DTOs
{
    public class ChatRoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsPrivate { get; set; }
        public int MaxMembers { get; set; }
        public int MemberCount { get; set; }
        public int UnreadMessageCount { get; set; }
        public ChatMessageDto? LastMessage { get; set; }
        public List<ChatRoomMemberDto> Members { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateChatRoomDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = "Group";
        public bool IsPrivate { get; set; } = false;
        public int MaxMembers { get; set; } = 100;
        public List<int> MemberIds { get; set; } = new();
    }

    public class ChatRoomMemberDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string? UserProfileImage { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public DateTime? LastReadAt { get; set; }
        public bool IsOnline { get; set; }
        public bool IsMuted { get; set; }
        public bool CanSendMessages { get; set; }
    }

    public class ChatMessageDto
    {
        public int Id { get; set; }
        public int ChatRoomId { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string? SenderProfileImage { get; set; }
        public string Content { get; set; } = string.Empty;
        public string MessageType { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public long? FileSize { get; set; }
        public int? ReplyToMessageId { get; set; }
        public ChatMessageDto? ReplyToMessage { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? EditedAt { get; set; }
        public List<MessageReactionDto> Reactions { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class SendMessageDto
    {
        public int ChatRoomId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string MessageType { get; set; } = "Text";
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public long? FileSize { get; set; }
        public int? ReplyToMessageId { get; set; }
    }

    public class EditMessageDto
    {
        public string Content { get; set; } = string.Empty;
    }

    public class MessageReactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Emoji { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Role { get; set; }
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeenAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; } = "User";
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
    }

    public class UpdateUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
        public bool IsActive { get; set; }
    }
}
