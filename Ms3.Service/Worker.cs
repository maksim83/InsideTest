using Common.Infrastructure.Utils;
using Common.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Ms3.Service.Services;
using OpenTracing;
using System.Threading;
using System.Threading.Tasks;

namespace Ms3.Service
{
    public class Worker : BackgroundService
    {

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

            var tracer = scope.ServiceProvider.GetRequiredService<ITracer>();
            var configuration = scope.ServiceProvider.GetService<IConfiguration>();

            using var _consumerBuilder = new ConsumerBuilder<Null, string>(scope.ServiceProvider.GetRequiredService<IOptions<ConsumerConfig>>().Value).Build();
            _consumerBuilder.Subscribe(configuration.GetSection("KafkaConfig").GetValue<string>("Topic"));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumerBuilder.Consume(stoppingToken).Message.Value;
                    _consumerBuilder.Commit();

                    if (!string.IsNullOrEmpty(consumeResult))
                    {
                        var message = await JsonHelper.GetObjectAsync<Message>(consumeResult);
                        await JaegerUtils.SetSpan(tracer, message, "Recieve from Ms2");
                        message.SetMs3Timestamp();
                        await _ms1ApiService.SendMessageAsync(message);
                    }
                }

                catch (ConsumeException)
                {
                }
                finally
                {                  

                }
            }
            _consumerBuilder.Close();
        }
    }
}

