using Common.Infrastructure.Utils;
using Common.Models;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketManager
{
    public class WebSocketHandler
    {
        protected ConnectionManager WebSocketConnectionManager { get; set; }

        public WebSocketHandler(ConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        public void Connect(string id, WebSocket socket)
        {
            WebSocketConnectionManager.AddSocket(id, socket);

        }

        public async Task Disconnect(WebSocket socket)
        {
            await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));
        }

        public async Task SendMessageAsync(string msName, Message message)
        {
            var socket = WebSocketConnectionManager.GetSocketById(msName);

            if (socket == null || socket.State != WebSocketState.Open)
                return;

            string jsonMessage = await JsonHelper.GetJsonAsync(message);
            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonMessage), 0, jsonMessage.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }



        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message), 0, message.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            foreach (var pair in WebSocketConnectionManager.GetAll())
            {
                if (pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }


    }
}