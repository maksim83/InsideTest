using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;


namespace WebSocketManager
{
    public class WebSocketManagerMiddleware
    {
        private WebSocketHandler webSocketHandler { get; set; }

        public WebSocketManagerMiddleware(RequestDelegate next,
                                          WebSocketHandler handler)
        {
            webSocketHandler = handler;
        }


        public async Task Invoke(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            StringValues id;
            context.Request.Query.TryGetValue("Id", out id);

            var socket = await context.WebSockets.AcceptWebSocketAsync();
            webSocketHandler.Connect(id, socket);

            await Receive(socket, async (result, buffer) =>
            {

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocketHandler.Disconnect(socket);
                    return;
                }
            });
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}