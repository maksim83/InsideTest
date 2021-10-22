using Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Ms2.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var buffer = new byte[1024 * 4];

            var client = new ClientWebSocket();
            await client.ConnectAsync(new Uri(_configuration.GetValue<string>("WsServiceUrl")), CancellationToken.None);

            while (!stoppingToken.IsCancellationRequested)
            {

                if (client.State != WebSocketState.Open)
                    await client.ConnectAsync(new Uri(_configuration.GetValue<string>("WsServiceUrl")), CancellationToken.None);

                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = JsonSerializer.Deserialize<Message>(Encoding.UTF8.GetString(buffer, 0, result.Count));

                    if (message != null)
                        message.SetMs2Timestamp();
                }

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }
            }
        }
    }
}

