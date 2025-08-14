using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvyyanBackend.DTOs;
using AvyyanBackend.Interfaces;
using System.Security.Claims;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        /// <summary>
        /// Get user's chat rooms
        /// </summary>
        [HttpGet("rooms")]
        public async Task<ActionResult<IEnumerable<ChatRoomDto>>> GetUserChatRooms()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var chatRooms = await _chatService.GetUserChatRoomsAsync(userId.Value);
                return Ok(chatRooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user chat rooms");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get specific chat room
        /// </summary>
        [HttpGet("rooms/{chatRoomId}")]
        public async Task<ActionResult<ChatRoomDto>> GetChatRoom(int chatRoomId)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                // Check if user is member of the room
                var isMember = await _chatService.IsUserMemberOfRoomAsync(userId.Value, chatRoomId);
                if (!isMember)
                    return Forbid("You are not a member of this chat room");

                var chatRoom = await _chatService.GetChatRoomAsync(chatRoomId);
                if (chatRoom == null)
                    return NotFound($"Chat room {chatRoomId} not found");

                return Ok(chatRoom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat room {ChatRoomId}", chatRoomId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create a new chat room
        /// </summary>
        [HttpPost("rooms")]
        public async Task<ActionResult<ChatRoomDto>> CreateChatRoom(CreateChatRoomDto createChatRoomDto)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var chatRoom = await _chatService.CreateChatRoomAsync(userId.Value, createChatRoomDto);
                return CreatedAtAction(nameof(GetChatRoom), new { chatRoomId = chatRoom.Id }, chatRoom);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating chat room");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get chat room messages
        /// </summary>
        [HttpGet("rooms/{chatRoomId}/messages")]
        public async Task<ActionResult<IEnumerable<ChatMessageDto>>> GetChatRoomMessages(
            int chatRoomId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                // Check if user is member of the room
                var isMember = await _chatService.IsUserMemberOfRoomAsync(userId.Value, chatRoomId);
                if (!isMember)
                    return Forbid("You are not a member of this chat room");

                var messages = await _chatService.GetChatRoomMessagesAsync(chatRoomId, page, pageSize);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting messages for chat room {ChatRoomId}", chatRoomId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get specific message
        /// </summary>
        [HttpGet("messages/{messageId}")]
        public async Task<ActionResult<ChatMessageDto>> GetMessage(int messageId)
        {
            try
            {
                var message = await _chatService.GetMessageAsync(messageId);
                if (message == null)
                    return NotFound($"Message {messageId} not found");

                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                // Check if user is member of the room
                var isMember = await _chatService.IsUserMemberOfRoomAsync(userId.Value, message.ChatRoomId);
                if (!isMember)
                    return Forbid("You are not a member of this chat room");

                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting message {MessageId}", messageId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Search users for adding to chat rooms
        /// </summary>
        [HttpGet("users/search")]
        public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsers([FromQuery] string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term cannot be empty");

                var users = await _chatService.SearchUsersAsync(searchTerm);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users with term {SearchTerm}", searchTerm);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get unread message counts for all user's chat rooms
        /// </summary>
        [HttpGet("unread-counts")]
        public async Task<ActionResult<Dictionary<int, int>>> GetUnreadMessageCounts()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var unreadCounts = await _chatService.GetUnreadMessageCountsAsync(userId.Value);
                return Ok(unreadCounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread message counts");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
