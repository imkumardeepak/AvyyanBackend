namespace AvyyanBackend.WebSockets;

using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using AvyyanBackend.Controllers;
using AvyyanBackend.Models;
using AvyyanBackend.DTOs;

public class ChatWebSocketManager
{
    private static readonly ConcurrentDictionary<string, WebSocket> _employeeSockets = new();
    private readonly ILogger<ChatWebSocketManager> _logger;
    private WebSocketManager _webSocketManager;

    public ChatWebSocketManager(ILogger<ChatWebSocketManager> logger, WebSocketManager webSocketManager)
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
    public async Task SendMessageToEmployee(string receiverId, string message)
    {
        if (_employeeSockets.TryGetValue(receiverId, out var socket) && socket.State == WebSocketState.Open)
        {
            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var messageSegment = new ArraySegment<byte>(messageBuffer);

            try
            {
                var statusMessage = $"Message Alert From someone";
                await _webSocketManager.SendNotification(statusMessage);


                await socket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
                _logger.LogInformation($"Sent private message to {receiverId}: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending to {receiverId}: {ex.Message}");
            }
        }
    }

    // **Broadcast message to all**
    public async Task BroadcastMessage(string message)
    {
        var messageBuffer = Encoding.UTF8.GetBytes(message);
        var messageSegment = new ArraySegment<byte>(messageBuffer);

        foreach (var socket in _employeeSockets.Values)
        {
            if (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error broadcasting message: {ex.Message}");
                }
            }
        }
    }
}