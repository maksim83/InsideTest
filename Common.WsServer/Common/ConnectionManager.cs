using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketManager
{
    public class ConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> sockets = new ConcurrentDictionary<string, WebSocket>();

        public WebSocket GetSocketById(string id)
        {
            return sockets.FirstOrDefault(p => p.Key == id).Value;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return sockets;
        }

        public string GetId(WebSocket socket)
        {
            return sockets.FirstOrDefault(p => p.Value == socket).Key;
        }
        public void AddSocket(string id, WebSocket socket)
        {
            sockets.TryAdd(id, socket);
        }

        public async Task RemoveSocket(string id)
        {
            WebSocket socket;
            sockets.TryRemove(id, out socket);

            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure, 
                                    statusDescription: "Closed by the ConnectionManager", 
                                    cancellationToken: CancellationToken.None);
        }

       
    }
}