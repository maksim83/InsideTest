using Common.Models;
using System.Threading.Tasks;

namespace Ms2.Service.Services
{
     interface IMessagePublishService
    {
        Task PublishMessage(Message message);
    }
}
