using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AvyyanBackend.DTOs;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;

namespace AvyyanBackend.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ChatRoom> _chatRoomRepository;
        private readonly IRepository<ChatMessage> _messageRepository;
        private readonly IRepository<ChatRoomMember> _memberRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserConnection> _connectionRepository;
        private readonly IRepository<MessageReaction> _reactionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ChatService> _logger;

        public ChatService(
            IUnitOfWork unitOfWork,
            IRepository<ChatRoom> chatRoomRepository,
            IRepository<ChatMessage> messageRepository,
            IRepository<ChatRoomMember> memberRepository,
            IRepository<User> userRepository,
            IRepository<UserConnection> connectionRepository,
            IRepository<MessageReaction> reactionRepository,
            IMapper mapper,
            ILogger<ChatService> logger)
        {
            _unitOfWork = unitOfWork;
            _chatRoomRepository = chatRoomRepository;
            _messageRepository = messageRepository;
            _memberRepository = memberRepository;
            _userRepository = userRepository;
            _connectionRepository = connectionRepository;
            _reactionRepository = reactionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ChatRoomDto> CreateChatRoomAsync(int creatorUserId, CreateChatRoomDto createChatRoomDto)
        {
            _logger.LogDebug("Creating chat room {Name} by user {UserId}", createChatRoomDto.Name, creatorUserId);

            var chatRoom = _mapper.Map<ChatRoom>(createChatRoomDto);
            chatRoom.CreatedByUserId = creatorUserId;

            await _chatRoomRepository.AddAsync(chatRoom);
            await _unitOfWork.SaveChangesAsync();

            // Add creator as admin member
            var creatorMember = new ChatRoomMember
            {
                ChatRoomId = chatRoom.Id,
                UserId = creatorUserId,
                Role = "Admin",
                JoinedAt = DateTime.UtcNow,
                CanSendMessages = true,
                CanInviteMembers = true
            };
            await _memberRepository.AddAsync(creatorMember);

            // Add other members
            foreach (var memberId in createChatRoomDto.MemberIds.Where(id => id != creatorUserId))
            {
                var member = new ChatRoomMember
                {
                    ChatRoomId = chatRoom.Id,
                    UserId = memberId,
                    Role = "Member",
                    JoinedAt = DateTime.UtcNow,
                    CanSendMessages = true
                };
                await _memberRepository.AddAsync(member);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Created chat room {ChatRoomId} with {MemberCount} members", chatRoom.Id, createChatRoomDto.MemberIds.Count + 1);

            return _mapper.Map<ChatRoomDto>(chatRoom);
        }

        public async Task<ChatRoomDto?> GetChatRoomAsync(int chatRoomId)
        {
            var chatRoom = await _chatRoomRepository.GetByIdAsync(chatRoomId);
            return chatRoom != null ? _mapper.Map<ChatRoomDto>(chatRoom) : null;
        }

        public async Task<IEnumerable<ChatRoomDto>> GetUserChatRoomsAsync(int userId)
        {
            var memberships = await _memberRepository.FindAsync(m => m.UserId == userId && m.IsActive);
            var chatRoomIds = memberships.Select(m => m.ChatRoomId).ToList();
            
            var chatRooms = await _chatRoomRepository.FindAsync(cr => chatRoomIds.Contains(cr.Id) && cr.IsActive);
            return _mapper.Map<IEnumerable<ChatRoomDto>>(chatRooms);
        }

        public async Task<bool> IsUserMemberOfRoomAsync(int userId, int chatRoomId)
        {
            return await _memberRepository.AnyAsync(m => m.UserId == userId && m.ChatRoomId == chatRoomId && m.IsActive);
        }

        public async Task<ChatMessageDto> SendMessageAsync(int senderId, SendMessageDto sendMessageDto)
        {
            _logger.LogDebug("User {UserId} sending message to room {ChatRoomId}", senderId, sendMessageDto.ChatRoomId);

            var message = _mapper.Map<ChatMessage>(sendMessageDto);
            message.SenderId = senderId;

            await _messageRepository.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Message {MessageId} sent to room {ChatRoomId}", message.Id, sendMessageDto.ChatRoomId);

            return _mapper.Map<ChatMessageDto>(message);
        }

        public async Task<ChatMessageDto?> GetMessageAsync(int messageId)
        {
            var message = await _messageRepository.GetByIdAsync(messageId);
            return message != null ? _mapper.Map<ChatMessageDto>(message) : null;
        }

        public async Task<IEnumerable<ChatMessageDto>> GetChatRoomMessagesAsync(int chatRoomId, int page = 1, int pageSize = 50)
        {
            var messages = await _messageRepository.FindAsync(m => 
                m.ChatRoomId == chatRoomId && 
                !m.IsDeleted && 
                m.IsActive);
            
            var pagedMessages = messages
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderBy(m => m.CreatedAt);

            return _mapper.Map<IEnumerable<ChatMessageDto>>(pagedMessages);
        }

        public async Task AddUserConnectionAsync(int userId, string connectionId, string userAgent, string ipAddress)
        {
            var connection = new UserConnection
            {
                UserId = userId,
                ConnectionId = connectionId,
                UserAgent = userAgent,
                IpAddress = ipAddress,
                ConnectedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _connectionRepository.AddAsync(connection);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogDebug("Added connection {ConnectionId} for user {UserId}", connectionId, userId);
        }

        public async Task RemoveUserConnectionAsync(string connectionId)
        {
            var connection = await _connectionRepository.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);
            if (connection != null)
            {
                connection.DisconnectedAt = DateTime.UtcNow;
                connection.IsActive = false;
                _connectionRepository.Update(connection);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogDebug("Removed connection {ConnectionId}", connectionId);
            }
        }

        public async Task<bool> HasActiveConnectionsAsync(int userId)
        {
            return await _connectionRepository.AnyAsync(c => c.UserId == userId && c.IsActive);
        }

        public async Task UpdateUserOnlineStatusAsync(int userId, bool isOnline)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.IsOnline = isOnline;
                if (!isOnline)
                {
                    user.LastSeenAt = DateTime.UtcNow;
                }
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogDebug("Updated user {UserId} online status to {IsOnline}", userId, isOnline);
            }
        }

        // Placeholder implementations for other interface methods
        public Task<ChatRoomDto?> UpdateChatRoomAsync(int chatRoomId, CreateChatRoomDto updateChatRoomDto) => throw new NotImplementedException();
        public Task<bool> DeleteChatRoomAsync(int chatRoomId) => throw new NotImplementedException();
        public Task<bool> AddMemberToChatRoomAsync(int chatRoomId, int userId, string role = "Member") => throw new NotImplementedException();
        public Task<bool> RemoveMemberFromChatRoomAsync(int chatRoomId, int userId) => throw new NotImplementedException();
        public Task<bool> UpdateMemberRoleAsync(int chatRoomId, int userId, string role) => throw new NotImplementedException();
        public Task<IEnumerable<ChatRoomMemberDto>> GetChatRoomMembersAsync(int chatRoomId) => throw new NotImplementedException();
        public Task<ChatMessageDto?> EditMessageAsync(int messageId, int userId, EditMessageDto editMessageDto) => throw new NotImplementedException();
        public Task<bool> DeleteMessageAsync(int messageId, int userId) => throw new NotImplementedException();
        public Task<bool> MarkMessageAsReadAsync(int userId, int messageId) => throw new NotImplementedException();
        public Task<bool> AddReactionAsync(int messageId, int userId, string emoji) => throw new NotImplementedException();
        public Task<bool> RemoveReactionAsync(int messageId, int userId, string emoji) => throw new NotImplementedException();
        public Task<IEnumerable<MessageReactionDto>> GetMessageReactionsAsync(int messageId) => throw new NotImplementedException();
        public Task<IEnumerable<ChatMessageDto>> SearchMessagesAsync(int chatRoomId, string searchTerm, int page = 1, int pageSize = 20) => throw new NotImplementedException();
        public Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm) => throw new NotImplementedException();
        public Task<ChatRoomDto?> GetOrCreatePersonalChatRoomAsync(int user1Id, int user2Id) => throw new NotImplementedException();
        public Task<int> GetUnreadMessageCountAsync(int userId, int chatRoomId) => throw new NotImplementedException();
        public Task<Dictionary<int, int>> GetUnreadMessageCountsAsync(int userId) => throw new NotImplementedException();
    }
}
