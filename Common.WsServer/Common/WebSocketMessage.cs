using System;
using System.Collections.Generic;

namespace Gis.WsServer.Common
{
    public class WebSocketMessage
    {
        public string SocketId { get; set; }
        public string MessageType { get; set; }        
        public DateTime Date { get; set; }
        public Dictionary<string, string> Params { get; set; }
    }
}
