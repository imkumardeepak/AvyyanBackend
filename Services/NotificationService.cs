using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using AvyyanBackend.DTOs;
using AvyyanBackend.Hubs;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;

namespace AvyyanBackend.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Notification> _notificationRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IUnitOfWork unitOfWork,
            IRepository<Notification> notificationRepository,
            IRepository<User> userRepository,
            IHubContext<NotificationHub> notificationHubContext,
            IMapper mapper,
            ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _notificationHubContext = notificationHubContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto createNotificationDto)
        {
            _logger.LogDebug("Creating notification for user {UserId}: {Title}", createNotificationDto.UserId, createNotificationDto.Title);

            var notification = _mapper.Map<Notification>(createNotificationDto);
            notification.SentAt = DateTime.UtcNow;

            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            var notificationDto = _mapper.Map<NotificationDto>(notification);

            // Send real-time notification
            await SendNotificationToUserAsync(createNotificationDto.UserId, notificationDto);

            _logger.LogInformation("Created notification {NotificationId} for user {UserId}", notification.Id, createNotificationDto.UserId);

            return notificationDto;
        }

        public async Task<IEnumerable<NotificationDto>> CreateBulkNotificationAsync(BulkNotificationDto bulkNotificationDto)
        {
            _logger.LogDebug("Creating bulk notification for {UserCount} users: {Title}", bulkNotificationDto.UserIds.Count, bulkNotificationDto.Title);

            var notifications = new List<Notification>();
            var notificationDtos = new List<NotificationDto>();

            foreach (var userId in bulkNotificationDto.UserIds)
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Title = bulkNotificationDto.Title,
                    Message = bulkNotificationDto.Message,
                    Type = bulkNotificationDto.Type,
                    Category = bulkNotificationDto.Category,
                    ActionUrl = bulkNotificationDto.ActionUrl,
                    ActionText = bulkNotificationDto.ActionText,
                    IsPush = bulkNotificationDto.IsPush,
                    IsEmail = bulkNotificationDto.IsEmail,
                    IsSms = bulkNotificationDto.IsSms,
                    ScheduledAt = bulkNotificationDto.ScheduledAt,
                    Metadata = bulkNotificationDto.Metadata,
                    SentAt = DateTime.UtcNow
                };

                notifications.Add(notification);
                notificationDtos.Add(_mapper.Map<NotificationDto>(notification));
            }

            await _notificationRepository.AddRangeAsync(notifications);
            await _unitOfWork.SaveChangesAsync();

            // Send real-time notifications
            await SendNotificationToUsersAsync(bulkNotificationDto.UserIds, notificationDtos.First());

            _logger.LogInformation("Created {NotificationCount} bulk notifications", notifications.Count);

            return notificationDtos;
        }

        public async Task<NotificationDto> CreateSystemNotificationAsync(string title, string message, string? actionUrl = null)
        {
            _logger.LogDebug("Creating system notification: {Title}", title);

            // Get all active users
            var users = await _userRepository.FindAsync(u => u.IsActive);
            var userIds = users.Select(u => u.Id).ToList();

            var bulkNotification = new BulkNotificationDto
            {
                UserIds = userIds,
                Title = title,
                Message = message,
                Type = "System",
                Category = "System",
                ActionUrl = actionUrl,
                IsPush = true
            };

            var notifications = await CreateBulkNotificationAsync(bulkNotification);
            return notifications.First();
        }

        public async Task<NotificationDto?> GetNotificationAsync(int notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            return notification != null ? _mapper.Map<NotificationDto>(notification) : null;
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsAsync(int userId, int page = 1, int pageSize = 20)
        {
            var notifications = await _notificationRepository.FindAsync(n => n.UserId == userId && n.IsActive);
            
            var pagedNotifications = notifications
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return _mapper.Map<IEnumerable<NotificationDto>>(pagedNotifications);
        }

        public async Task<IEnumerable<NotificationDto>> GetRecentNotificationsAsync(int userId, int count = 10)
        {
            var notifications = await _notificationRepository.FindAsync(n => n.UserId == userId && n.IsActive);
            
            var recentNotifications = notifications
                .OrderByDescending(n => n.CreatedAt)
                .Take(count);

            return _mapper.Map<IEnumerable<NotificationDto>>(recentNotifications);
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.FindAsync(n => 
                n.UserId == userId && 
                !n.IsRead && 
                n.IsActive);

            return _mapper.Map<IEnumerable<NotificationDto>>(notifications.OrderByDescending(n => n.CreatedAt));
        }

        public async Task<bool> MarkAsReadAsync(int userId, int notificationId)
        {
            var notification = await _notificationRepository.FirstOrDefaultAsync(n => 
                n.Id == notificationId && 
                n.UserId == userId);

            if (notification == null) return false;

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _notificationRepository.Update(notification);
            
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            var unreadNotifications = await _notificationRepository.FindAsync(n => 
                n.UserId == userId && 
                !n.IsRead && 
                n.IsActive);

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            _notificationRepository.UpdateRange(unreadNotifications);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteNotificationAsync(int userId, int notificationId)
        {
            var notification = await _notificationRepository.FirstOrDefaultAsync(n => 
                n.Id == notificationId && 
                n.UserId == userId);

            if (notification == null) return false;

            notification.IsActive = false;
            _notificationRepository.Update(notification);
            
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            return await _notificationRepository.CountAsync(n => 
                n.UserId == userId && 
                !n.IsRead && 
                n.IsActive);
        }

        public async Task SendNotificationToUserAsync(int userId, NotificationDto notification)
        {
            try
            {
                await _notificationHubContext.Clients.Group($"User_{userId}")
                    .SendAsync("ReceiveNotification", notification);

                _logger.LogDebug("Sent real-time notification to user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send real-time notification to user {UserId}", userId);
            }
        }

        public async Task SendNotificationToUsersAsync(IEnumerable<int> userIds, NotificationDto notification)
        {
            try
            {
                var tasks = userIds.Select(userId => SendNotificationToUserAsync(userId, notification));
                await Task.WhenAll(tasks);

                _logger.LogDebug("Sent real-time notification to {UserCount} users", userIds.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send real-time notifications to multiple users");
            }
        }

        public async Task SendChatNotificationAsync(int userId, int chatRoomId, string senderName, string message)
        {
            var notification = new CreateNotificationDto
            {
                UserId = userId,
                Title = $"New message from {senderName}",
                Message = message.Length > 100 ? message.Substring(0, 100) + "..." : message,
                Type = "Chat",
                Category = "Chat",
                ActionUrl = $"/chat/{chatRoomId}",
                ActionText = "View Chat",
                RelatedEntityId = chatRoomId,
                RelatedEntityType = "ChatRoom"
            };

            await CreateNotificationAsync(notification);
        }

        // Placeholder implementations for other interface methods
        public Task<bool> DeleteAllNotificationsAsync(int userId) => throw new NotImplementedException();
        public Task<NotificationStatsDto> GetNotificationStatsAsync(int userId) => throw new NotImplementedException();
        public Task<NotificationSettingsDto> GetNotificationSettingsAsync(int userId) => throw new NotImplementedException();
        public Task<NotificationSettingsDto> UpdateNotificationSettingsAsync(int userId, NotificationSettingsDto settings) => throw new NotImplementedException();
        public Task SendNotificationToAllUsersAsync(NotificationDto notification) => throw new NotImplementedException();
        public Task SendNotificationToRoleAsync(string role, NotificationDto notification) => throw new NotImplementedException();
        public Task SendOrderNotificationAsync(int userId, int orderId, string type, string message) => throw new NotImplementedException();
        public Task SendProductNotificationAsync(int userId, int productId, string type, string message) => throw new NotImplementedException();
        public Task SendSystemMaintenanceNotificationAsync(DateTime scheduledTime, string message) => throw new NotImplementedException();
        public Task<IEnumerable<NotificationDto>> GetScheduledNotificationsAsync() => throw new NotImplementedException();
        public Task ProcessScheduledNotificationsAsync() => throw new NotImplementedException();
        public Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync(int page = 1, int pageSize = 50) => throw new NotImplementedException();
        public Task<NotificationStatsDto> GetGlobalNotificationStatsAsync() => throw new NotImplementedException();
        public Task<bool> DeleteNotificationByIdAsync(int notificationId) => throw new NotImplementedException();
    }
}
