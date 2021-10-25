using Common.Infrastructure.Utils;
using Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ms2.Service.Services;
using Ms3.Service.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Ms3.Service
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IMessageConsumeService _messageConsumeService;
        private readonly IServiceScopeFactory _scopeFactory;
        private IMs1ApiService _ms1ApiService;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using var scope = _scopeFactory.CreateScope();
            _ms1ApiService = scope.ServiceProvider.GetRequiredService<IMs1ApiService>();
            _messageConsumeService = scope.ServiceProvider.GetRequiredService<IMessageConsumeService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _messageConsumeService.ConsumeMessage(stoppingToken);

                if (!string.IsNullOrEmpty(consumeResult))
                {
                    var message = await JsonHelper.GetObjectAsync<Message>(consumeResult);
                    message.SetMs3Timestamp();
                    await _ms1ApiService.SendMessageAsync(message);
                }

            }
        }
    }
}
