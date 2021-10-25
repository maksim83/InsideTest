using Common.Infrastructure.Utils;
using Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ms2.Service.Services;
using Ms3.Service.Services;
using OpenTracing;
using System.Threading;
using System.Threading.Tasks;

namespace Ms3.Service
{
    public class Worker : BackgroundService
    {

        private IMessageConsumeService _messageConsumeService;
        private readonly IServiceScopeFactory _scopeFactory;
        private IMs1ApiService _ms1ApiService;

        public Worker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            using var scope = _scopeFactory.CreateScope();
            _ms1ApiService = scope.ServiceProvider.GetRequiredService<IMs1ApiService>();
            _messageConsumeService = scope.ServiceProvider.GetRequiredService<IMessageConsumeService>();

            var tracer = scope.ServiceProvider.GetRequiredService<ITracer>();

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _messageConsumeService.ConsumeMessage(stoppingToken);

                if (!string.IsNullOrEmpty(consumeResult))
                {
                    var message = await JsonHelper.GetObjectAsync<Message>(consumeResult);

                    await JaegerUtils.SetSpan(tracer, message, "Recieve from Ms2");

                    message.SetMs3Timestamp();
                    await _ms1ApiService.SendMessageAsync(message);
                }

            }
        }
    }
}
