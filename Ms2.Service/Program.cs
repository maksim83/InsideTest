using Common.Infrastructure.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ms2.Service.Services;
using System.Reflection;

namespace Ms2.Service
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
                    services.AddScoped(typeof(IMessagePublishService), typeof(MessagePublishService));
                    JaegerUtils.ConfigureService(services, Assembly.GetEntryAssembly().GetName().Name);
                });
    }
}
