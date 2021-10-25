using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common.Infrastructure.Utils
{
    public static class JsonHelper
    {
        public async static Task<string> GetJsonAsync<T>(T message)
        {
            using var stream = new MemoryStream();

            await JsonSerializer.SerializeAsync(stream, message);
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        public async static Task<T> GetObjectAsync<T>(string streamString)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(streamString);
            using var memoryStream = new MemoryStream(byteArray);
            return await JsonSerializer.DeserializeAsync<T>(memoryStream);
        }
    }
}
