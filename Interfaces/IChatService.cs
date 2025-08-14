using AvyyanBackend.DTOs;

namespace AvyyanBackend.Interfaces
{
    public interface IChatService
    {
        // Chat Room Management
        Task<ChatRoomDto> CreateChatRoomAsync(int creatorUserId, CreateChatRoomDto createChatRoomDto);
        Task<ChatRoomDto?> GetChatRoomAsync(int chatRoomId);
        Task<IEnumerable<ChatRoomDto>> GetUserChatRoomsAsync(int userId);
        Task<ChatRoomDto?> UpdateChatRoomAsync(int chatRoomId, CreateChatRoomDto updateChatRoomDto);
        Task<bool> DeleteChatRoomAsync(int chatRoomId);
        
        // Chat Room Members
        Task<bool> AddMemberToChatRoomAsync(int chatRoomId, int userId, string role = "Member");
        Task<bool> RemoveMemberFromChatRoomAsync(int chatRoomId, int userId);
        Task<bool> UpdateMemberRoleAsync(int chatRoomId, int userId, string role);
        Task<IEnumerable<ChatRoomMemberDto>> GetChatRoomMembersAsync(int chatRoomId);
        Task<bool> IsUserMemberOfRoomAsync(int userId, int chatRoomId);
        
        // Messages
        Task<ChatMessageDto> SendMessageAsync(int senderId, SendMessageDto sendMessageDto);
        Task<ChatMessageDto?> GetMessageAsync(int messageId);
        Task<IEnumerable<ChatMessageDto>> GetChatRoomMessagesAsync(int chatRoomId, int page = 1, int pageSize = 50);
        Task<ChatMessageDto?> EditMessageAsync(int messageId, int userId, EditMessageDto editMessageDto);
        Task<bool> DeleteMessageAsync(int messageId, int userId);
        Task<bool> MarkMessageAsReadAsync(int userId, int messageId);
        
        // Message Reactions
        Task<bool> AddReactionAsync(int messageId, int userId, string emoji);
        Task<bool> RemoveReactionAsync(int messageId, int userId, string emoji);
        Task<IEnumerable<MessageReactionDto>> GetMessageReactionsAsync(int messageId);
        
        // User Connections
        Task AddUserConnectionAsync(int userId, string connectionId, string userAgent, string ipAddress);
        Task RemoveUserConnectionAsync(string connectionId);
        Task<bool> HasActiveConnectionsAsync(int userId);
        Task UpdateUserOnlineStatusAsync(int userId, bool isOnline);
        
        // Search and Utilities
        Task<IEnumerable<ChatMessageDto>> SearchMessagesAsync(int chatRoomId, string searchTerm, int page = 1, int pageSize = 20);
        Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm);
        Task<ChatRoomDto?> GetOrCreatePersonalChatRoomAsync(int user1Id, int user2Id);
        Task<int> GetUnreadMessageCountAsync(int userId, int chatRoomId);
        Task<Dictionary<int, int>> GetUnreadMessageCountsAsync(int userId);
    }
}
