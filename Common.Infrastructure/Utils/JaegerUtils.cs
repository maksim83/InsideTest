﻿using Common.Models;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using System.Threading.Tasks;

namespace Common.Infrastructure.Utils
{
    public static class JaegerUtils
    {
        public static void ConfigureService(IServiceCollection services, string serviceName)
        {
            services.AddOpenTracing();

            services.AddSingleton(serviceProvider =>
            {


                ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                ISampler sampler = new ConstSampler(sample: true);

                var reporter = new RemoteReporter.Builder()
                .WithLoggerFactory(loggerFactory)
                .WithSender(new UdpSender("172.22.2.204", 6831, 0))
                .Build();

                ITracer tracer = new Tracer.Builder(serviceName)
                    .WithLoggerFactory(loggerFactory)
                    .WithSampler(sampler)
                    .WithReporter(reporter)
                    .Build();


                return tracer;
            });
        }

        public async static Task SetSpan(ITracer tracer, Message message, string operationName)
        {
            using var scope = tracer.BuildSpan(operationName).StartActive(true);
            scope.Span.SetTag("Message", await JsonHelper.GetJsonAsync(message));
        }
    }

}
