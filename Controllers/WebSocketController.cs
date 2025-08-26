using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net.WebSockets;
using AvyyanBackend.WebSockets;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebSocketController : ControllerBase
    {
        private readonly CustomWebSocketManager _webSocketManager;
        private readonly ChatWebSocketManager _chatWebSocketManager;

        public WebSocketController(CustomWebSocketManager webSocketManager, ChatWebSocketManager chatWebSocketManager)
        {
            _webSocketManager = webSocketManager;
            _chatWebSocketManager = chatWebSocketManager;
        }

        [HttpGet("/ws")]
        public async Task GetWebSocket()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _webSocketManager.HandleWebSocketConnection(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        [HttpGet("/ws/chat/{employeeId}")]
        public async Task GetChatWebSocket(string employeeId)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                if (!string.IsNullOrEmpty(employeeId))
                {
                    var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                    await _chatWebSocketManager.HandleWebSocketConnection(employeeId, webSocket);
                }
                else
                {
                    HttpContext.Response.StatusCode = 400;
                }
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        [HttpGet("/api/notification")]
        public async Task<IActionResult> SendNotification()
        {
            await _webSocketManager.SendNotification("Hello from the API!");
            return Ok("Notification sent.");
        }
    }
}