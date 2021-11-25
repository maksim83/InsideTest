using Common.Infrastructure.Utils;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ms3.Service.Services;
using System;
using System.Reflection;

namespace Ms3.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    services.Configure<ConsumerConfig>(consumerConfig =>
                    {
                        var _configuration = hostContext.Configuration;
                        consumerConfig.GroupId = _configuration.GetSection("KafkaConfig").GetValue<string>("GroupId");
                        consumerConfig.BootstrapServers = _configuration.GetSection("KafkaConfig").GetValue<string>("Url");
                        consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
                        consumerConfig.AutoCommitIntervalMs = 0;

                    });

                    services.AddHttpClient<IMs1ApiService, Ms1ApiService>("Ms1Api",
                         client =>
                         {
                             client.BaseAddress = new Uri(hostContext.Configuration.GetValue<string>("Ms1ApiServiceUrl"));
                         });

                    JaegerUtils.ConfigureService(services, Assembly.GetEntryAssembly().GetName().Name, hostContext.Configuration);
                });
    }
}
