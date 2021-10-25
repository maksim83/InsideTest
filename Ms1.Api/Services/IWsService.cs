using Common.Models;
using System.Threading.Tasks;

namespace Ms1.Api.Services
{
    public interface IWsService
    {
        public  Task<Message> SendMessageAsync(long sessionId);
    }
}
