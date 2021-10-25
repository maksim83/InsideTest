using Common.Infrastructure.Utils;
using Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ms2.Service.Services;
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
        private readonly IServiceScopeFactory _scopeFactory;
        private ClientWebSocket _webSocketClient = new ClientWebSocket();

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        private async Task InitWebSoketClient()
        {
            await _webSocketClient.ConnectAsync(new Uri(_configuration.GetValue<string>("WsServiceUrl")), CancellationToken.None);

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var buffer = new byte[1024 * 4];

            using var scope = _scopeFactory.CreateScope();
            var messagePublishService = scope.ServiceProvider.GetRequiredService<IMessagePublishService>();

            while (!stoppingToken.IsCancellationRequested)
            {

                if (_webSocketClient.State != WebSocketState.Open)
                    await InitWebSoketClient();

                var result = await _webSocketClient.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message =  await JsonHelper.GetObjectAsync<Message>(Encoding.UTF8.GetString(buffer, 0, result.Count));

                    if (message != null)
                    {
                        message.SetMs2Timestamp();
                        await messagePublishService.PublishMessage(message);
                    }
                }

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocketClient.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }
            }
        }
    }
}

