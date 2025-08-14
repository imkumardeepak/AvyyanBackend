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
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Get user's notifications
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var notifications = await _notificationService.GetNotificationsAsync(userId.Value, page, pageSize);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notifications");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get unread notifications
        /// </summary>
        [HttpGet("unread")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUnreadNotifications()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var notifications = await _notificationService.GetUnreadNotificationsAsync(userId.Value);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notifications");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get unread notification count
        /// </summary>
        [HttpGet("unread/count")]
        public async Task<ActionResult<int>> GetUnreadNotificationCount()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var count = await _notificationService.GetUnreadNotificationCountAsync(userId.Value);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notification count");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get recent notifications
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetRecentNotifications([FromQuery] int count = 10)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var notifications = await _notificationService.GetRecentNotificationsAsync(userId.Value, count);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent notifications");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Mark notification as read
        /// </summary>
        [HttpPatch("{notificationId}/read")]
        public async Task<ActionResult> MarkAsRead(int notificationId)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var result = await _notificationService.MarkAsReadAsync(userId.Value, notificationId);
                if (!result)
                    return NotFound($"Notification {notificationId} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        [HttpPatch("read-all")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                await _notificationService.MarkAllAsReadAsync(userId.Value);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete notification
        /// </summary>
        [HttpDelete("{notificationId}")]
        public async Task<ActionResult> DeleteNotification(int notificationId)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var result = await _notificationService.DeleteNotificationAsync(userId.Value, notificationId);
                if (!result)
                    return NotFound($"Notification {notificationId} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create notification (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<NotificationDto>> CreateNotification(CreateNotificationDto createNotificationDto)
        {
            try
            {
                var notification = await _notificationService.CreateNotificationAsync(createNotificationDto);
                return CreatedAtAction(nameof(GetNotification), new { notificationId = notification.Id }, notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create bulk notification (Admin only)
        /// </summary>
        [HttpPost("bulk")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> CreateBulkNotification(BulkNotificationDto bulkNotificationDto)
        {
            try
            {
                var notifications = await _notificationService.CreateBulkNotificationAsync(bulkNotificationDto);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bulk notification");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create system notification (Admin only)
        /// </summary>
        [HttpPost("system")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<NotificationDto>> CreateSystemNotification([FromBody] CreateSystemNotificationRequest request)
        {
            try
            {
                var notification = await _notificationService.CreateSystemNotificationAsync(request.Title, request.Message, request.ActionUrl);
                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating system notification");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get specific notification
        /// </summary>
        [HttpGet("{notificationId}")]
        public async Task<ActionResult<NotificationDto>> GetNotification(int notificationId)
        {
            try
            {
                var notification = await _notificationService.GetNotificationAsync(notificationId);
                if (notification == null)
                    return NotFound($"Notification {notificationId} not found");

                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification {NotificationId}", notificationId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public class CreateSystemNotificationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ActionUrl { get; set; }
    }
}
