using Common.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ms3.Service.Services
{
    class Ms1ApiService : IMs1ApiService
    {
        private readonly HttpClient _httpClient;

        public Ms1ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> SendMessageAsync(Message message)
        {
            return await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress, message);
        }
    }
}
