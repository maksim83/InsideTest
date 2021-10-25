using Common.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ms3.Service.Services
{
    interface IMs1ApiService
    {
        public Task<HttpResponseMessage> SendMessageAsync(Message message);
    }
}
