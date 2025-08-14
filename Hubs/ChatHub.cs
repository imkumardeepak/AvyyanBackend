using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using AvyyanBackend.Interfaces;
using AvyyanBackend.DTOs;
using System.Security.Claims;

namespace AvyyanBackend.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatService chatService, INotificationService notificationService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                _logger.LogInformation("User {UserId} connected with connection {ConnectionId}", userId, Context.ConnectionId);
                
                // Store connection
                await _chatService.AddUserConnectionAsync(userId.Value, Context.ConnectionId, GetUserAgent(), GetIpAddress());
                
                // Update user online status
                await _chatService.UpdateUserOnlineStatusAsync(userId.Value, true);
                
                // Join user to their personal rooms
                var userRooms = await _chatService.GetUserChatRoomsAsync(userId.Value);
                foreach (var room in userRooms)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"ChatRoom_{room.Id}");
                }
                
                // Notify contacts that user is online
                await Clients.Others.SendAsync("UserOnline", new { UserId = userId.Value, ConnectionId = Context.ConnectionId });
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                _logger.LogInformation("User {UserId} disconnected from connection {ConnectionId}", userId, Context.ConnectionId);
                
                // Remove connection
                await _chatService.RemoveUserConnectionAsync(Context.ConnectionId);
                
                // Check if user has other active connections
                var hasOtherConnections = await _chatService.HasActiveConnectionsAsync(userId.Value);
                if (!hasOtherConnections)
                {
                    // Update user offline status
                    await _chatService.UpdateUserOnlineStatusAsync(userId.Value, false);
                    
                    // Notify contacts that user is offline
                    await Clients.Others.SendAsync("UserOffline", new { UserId = userId.Value });
                }
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(SendMessageDto messageDto)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                {
                    await Clients.Caller.SendAsync("Error", "User not authenticated");
                    return;
                }

                _logger.LogDebug("User {UserId} sending message to room {ChatRoomId}", userId, messageDto.ChatRoomId);

                // Validate user is member of the chat room
                var isMember = await _chatService.IsUserMemberOfRoomAsync(userId.Value, messageDto.ChatRoomId);
                if (!isMember)
                {
                    await Clients.Caller.SendAsync("Error", "You are not a member of this chat room");
                    return;
                }

                // Save message to database
                var message = await _chatService.SendMessageAsync(userId.Value, messageDto);

                // Send message to all room members
                await Clients.Group($"ChatRoom_{messageDto.ChatRoomId}").SendAsync("ReceiveMessage", message);

                _logger.LogInformation("Message {MessageId} sent to room {ChatRoomId}", message.Id, messageDto.ChatRoomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                await Clients.Caller.SendAsync("Error", "Failed to send message");
            }
        }

        public async Task JoinRoom(int chatRoomId)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                {
                    await Clients.Caller.SendAsync("Error", "User not authenticated");
                    return;
                }

                var isMember = await _chatService.IsUserMemberOfRoomAsync(userId.Value, chatRoomId);
                if (!isMember)
                {
                    await Clients.Caller.SendAsync("Error", "You are not a member of this chat room");
                    return;
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, $"ChatRoom_{chatRoomId}");
                await Clients.Caller.SendAsync("JoinedRoom", chatRoomId);
                
                _logger.LogDebug("User {UserId} joined room {ChatRoomId}", userId, chatRoomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining room {ChatRoomId}", chatRoomId);
                await Clients.Caller.SendAsync("Error", "Failed to join room");
            }
        }

        public async Task LeaveRoom(int chatRoomId)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"ChatRoom_{chatRoomId}");
                await Clients.Caller.SendAsync("LeftRoom", chatRoomId);
                
                var userId = GetUserId();
                _logger.LogDebug("User {UserId} left room {ChatRoomId}", userId, chatRoomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving room {ChatRoomId}", chatRoomId);
            }
        }

        public async Task MarkMessageAsRead(int messageId)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue) return;

                await _chatService.MarkMessageAsReadAsync(userId.Value, messageId);
                
                // Notify sender that message was read
                var message = await _chatService.GetMessageAsync(messageId);
                if (message != null)
                {
                    await Clients.Group($"ChatRoom_{message.ChatRoomId}").SendAsync("MessageRead", new { MessageId = messageId, UserId = userId.Value });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking message as read");
            }
        }

        public async Task StartTyping(int chatRoomId)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue) return;

                await Clients.OthersInGroup($"ChatRoom_{chatRoomId}").SendAsync("UserTyping", new { UserId = userId.Value, ChatRoomId = chatRoomId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in StartTyping");
            }
        }

        public async Task StopTyping(int chatRoomId)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue) return;

                await Clients.OthersInGroup($"ChatRoom_{chatRoomId}").SendAsync("UserStoppedTyping", new { UserId = userId.Value, ChatRoomId = chatRoomId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in StopTyping");
            }
        }

        private int? GetUserId()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        private string GetUserAgent()
        {
            return Context.GetHttpContext()?.Request.Headers.UserAgent.FirstOrDefault() ?? "Unknown";
        }

        private string GetIpAddress()
        {
            return Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
