namespace AvyyanBackend.WebSockets;

// WebSocketManager.cs
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

public class WebSocketManager
{
    private static readonly ConcurrentBag<WebSocket> _sockets = new();
    private readonly ILogger<WebSocketManager> _logger;

    public WebSocketManager(ILogger<WebSocketManager> logger)
    {
        _logger = logger;
    }

    public async Task HandleWebSocketConnection(WebSocket webSocket)
    {
        _sockets.Add(webSocket);
        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _sockets.TryTake(out _);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    _logger.LogInformation("WebSocket connection closed normally.");
                    break; // Exit the loop after closing.
                }
                else if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    _logger.LogInformation($"Received message: {message}");
                    // Process the received message (e.g., echo back, store, etc.)

                    // Example: Echo back the message (optional)
                    // await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"WebSocket exception: {ex.Message}");
            // Handle exceptions such as client disconnecting abruptly.
        }
        finally
        {
            if (webSocket.State != WebSocketState.Closed)
            {
                try
                {
                    _sockets.TryTake(out _);
                    await webSocket.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, "Connection ended", CancellationToken.None);
                }
                catch (Exception closeEx)
                {
                    _logger.LogError($"Error closing WebSocket: {closeEx.Message}");
                }
            }
        }
    }

    public async Task SendNotification(string message)
    {
        var messageBuffer = Encoding.UTF8.GetBytes(message);
        var messageSegment = new ArraySegment<byte>(messageBuffer);

        foreach (var socket in _sockets)
        {
            if (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
                    _logger.LogInformation($"Sent notification: {message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending message to WebSocket: {ex.Message}");
                    // Remove the socket if sending fails
                    _sockets.TryTake(out _);
                    try { socket.Abort(); } catch { } //Try to abort it.
                }
            }
        }
    }
}