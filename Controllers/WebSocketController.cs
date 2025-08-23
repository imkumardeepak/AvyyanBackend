using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using AvyyanBackend.WebSockets;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("/ws")]
    public class WebSocketController : ControllerBase
    {
        private readonly AvyyanBackend.WebSockets.WebSocketManager _webSocketManager;
        private readonly ChatWebSocketManager _chatWebSocketManager;

        public WebSocketController(AvyyanBackend.WebSockets.WebSocketManager webSocketManager, ChatWebSocketManager chatWebSocketManager)
        {
            _webSocketManager = webSocketManager;
            _chatWebSocketManager = chatWebSocketManager;
        }

        [HttpGet]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _webSocketManager.HandleWebSocketConnection(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        [HttpGet("chat/{employeeId}")]
        public async Task GetChat(string employeeId)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _chatWebSocketManager.HandleWebSocketConnection(employeeId, webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }
    }
}