using Common.Infrastructure.Utils;
using Common.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Ms2.Service.Services
{
    class MessageConsumeService : IMessageConsumeService
    {
        private readonly IConfiguration _configuration;
        private readonly ConsumerConfig _consumerConfig;
        private readonly string _messageTopic;


        public MessageConsumeService(IConfiguration configuration)
        {
            _configuration = configuration;
            _consumerConfig = new ConsumerConfig
            {
                GroupId = _configuration.GetSection("KafkaConfig").GetValue<string>("GroupId"),
                BootstrapServers = _configuration.GetSection("KafkaConfig").GetValue<string>("Url"),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true

            };
            _messageTopic = _configuration.GetSection("KafkaConfig").GetValue<string>("Topic");
        }

        public string ConsumeMessage(CancellationToken stoppingToken)
        {
            using var builder = new ConsumerBuilder<Null, string>(_consumerConfig).Build();
            builder.Subscribe(_messageTopic);

            var result = builder.Consume(stoppingToken).Message.Value;
            builder.Commit();

            return result;


        }

    }
}
