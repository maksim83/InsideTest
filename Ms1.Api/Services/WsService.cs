using Common.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ms1.Api.Services
{
    public class WsService : IWsService
    {
        private readonly HttpClient _client;

        public WsService(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> SendMessage()
        {
            return await _client.PostAsJsonAsync(_client.BaseAddress, new Message());
        }
    }
}
