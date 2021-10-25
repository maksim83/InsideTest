using Common.Infrastructure.Utils;
using Common.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Ms2.Service.Services
{
    class MessagePublishService : IMessagePublishService
    {
        private readonly IConfiguration _configuration;
        private readonly ProducerConfig _producerConfig;
        private readonly string _messageTopic;

        public MessagePublishService(IConfiguration configuration)
        {
            _configuration = configuration;
            _producerConfig = new ProducerConfig
            {
                BootstrapServers = _configuration.GetSection("KafkaConfig").GetValue<string>("Url"),
                SecurityProtocol = SecurityProtocol.Plaintext,

            };
            _messageTopic = _configuration.GetSection("KafkaConfig").GetValue<string>("Topic");

        }

        public async Task PublishMessage(Message message)
        {
            using var producer = new ProducerBuilder<Null, string>(_producerConfig).Build();
            await producer.ProduceAsync(_messageTopic, new Message<Null, string> { Value = await JsonHelper.GetJsonAsync(message) });
        }


    }
}
