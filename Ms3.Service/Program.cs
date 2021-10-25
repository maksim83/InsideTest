using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ms2.Service.Services;
using Ms3.Service.Services;
using System;

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
                    services.AddScoped(typeof(IMessageConsumeService), typeof(MessageConsumeService));
                    services.AddHttpClient<IMs1ApiService, Ms1ApiService>("Ms1Api",
             client =>
             {
                 client.BaseAddress = new Uri(hostContext.Configuration.GetValue<string>("Ms1ApiServiceUrl"));
             });



                });
    }
}
