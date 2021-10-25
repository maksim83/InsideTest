using Common.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ms1.Api.Services
{
    public class WsService : IWsService
    {
        private readonly HttpClient _httpClient;

        public WsService(HttpClient client)
        {
            _httpClient = client;
        }

        public async Task<Message> SendMessageAsync(long sessionId)
        {
            var message = new Message();
            message.SetSessionId(sessionId);

             await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress, message);
            return message;
        }
    }
}
