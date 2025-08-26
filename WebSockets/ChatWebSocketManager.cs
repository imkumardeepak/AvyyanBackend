using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AvyyanBackend.WebSockets
{
	public class ChatWebSocketManager
	{
		private static readonly ConcurrentDictionary<string, WebSocket> _employeeSockets = new();
		private readonly ILogger<ChatWebSocketManager> _logger;
		private CustomWebSocketManager _webSocketManager;

		public ChatWebSocketManager(ILogger<ChatWebSocketManager> logger, CustomWebSocketManager webSocketManager)
		{
			_logger = logger;
			_webSocketManager = webSocketManager;
		}

		public async Task HandleWebSocketConnection(string employeeId, WebSocket webSocket)
		{
			_employeeSockets[employeeId] = webSocket;
			var buffer = new byte[1024 * 4];

			try
			{
				while (webSocket.State == WebSocketState.Open)
				{
					var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

					if (result.MessageType == WebSocketMessageType.Close)
					{
						_employeeSockets.TryRemove(employeeId, out _);
						await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
						_logger.LogInformation($"Employee {employeeId} disconnected.");
						break;
					}
					else if (result.MessageType == WebSocketMessageType.Text)
					{
						var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
						_logger.LogInformation($"Received from {employeeId}: {message}");

						ChatMessage chatMessage;
						try
						{
							chatMessage = JsonSerializer.Deserialize<ChatMessage>(message) ?? new ChatMessage();
							chatMessage.SenderId = employeeId; // Set the sender id
						}
						catch (JsonException ex)
						{
							_logger.LogError($"Invalid message format from {employeeId}: {ex.Message}");
							continue;
						}

						// **Group chat: Broadcast to all**
						if (string.IsNullOrEmpty(chatMessage.ReceiverId))
						{
							await BroadcastMessage(chatMessage);
						}
						// **Private chat: Send to specific employee**
						else
						{
							await SendMessageToEmployee(chatMessage.ReceiverId, chatMessage);
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error with employee {employeeId}: {ex.Message}");
			}
			finally
			{
				_employeeSockets.TryRemove(employeeId, out _);
				if (webSocket.State != WebSocketState.Closed)
					await webSocket.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "Connection ended", CancellationToken.None);
			}
		}


		// **Send private message**
		public async Task SendMessageToEmployee(string receiverId, ChatMessage message)
		{
			if (_employeeSockets.TryGetValue(receiverId, out var socket) && socket.State == WebSocketState.Open)
			{
				var messageJson = JsonSerializer.Serialize(message);
				var messageBuffer = Encoding.UTF8.GetBytes(messageJson);
				var messageSegment = new ArraySegment<byte>(messageBuffer);

				try
				{
					var statusMessage = $"Message Alert From {message.SenderName ?? "Unknown"}";
					await _webSocketManager.SendNotification(statusMessage);
					await socket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
					_logger.LogInformation($"Sent private message to {receiverId}: {messageJson}");
				}
				catch (Exception ex)
				{
					_logger.LogError($"Error sending to {receiverId}: {ex.Message}");
					_employeeSockets.TryRemove(receiverId, out _);
					try { socket.Abort(); } catch { }
				}
			}
		}

		// **Broadcast message to all employees**
		public async Task BroadcastMessage(ChatMessage message)
		{
			_logger.LogInformation($"Broadcasting message: {JsonSerializer.Serialize(message)}");

			var messageJson = JsonSerializer.Serialize(message);
			var messageBuffer = Encoding.UTF8.GetBytes(messageJson);
			var messageSegment = new ArraySegment<byte>(messageBuffer);

			foreach (var (employeeId, socket) in _employeeSockets)
			{
				if (socket.State == WebSocketState.Open)
				{
					try
					{
						await socket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
						var statusMessage = $"Group Chat Message From {message.SenderName ?? "Unknown"}";
						await _webSocketManager.SendNotification(statusMessage);
						_logger.LogInformation($"Broadcasted message to {employeeId}: {messageJson}");
					}
					catch (Exception ex)
					{
						_logger.LogError($"Error broadcasting to {employeeId}: {ex.Message}");
						_employeeSockets.TryRemove(employeeId, out _);
						try { socket.Abort(); } catch { }
					}
				}
			}
		}

	}

	// **Message Model**
	public class ChatMessage
	{
		public string? SenderId { get; set; } = "1";
		public string? SenderName { get; set; } = "DEEPAK";
		public string? Message { get; set; } = "HII";
		public string? ReceiverId { get; set; } // Null for group chat, set for private messages
	}
}