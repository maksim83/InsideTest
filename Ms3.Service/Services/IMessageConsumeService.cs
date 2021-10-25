using Common.Models;
using System.Threading;


namespace Ms2.Service.Services
{
   public  interface IMessageConsumeService
    {
        string ConsumeMessage(CancellationToken stoppingToken);
    }
}
