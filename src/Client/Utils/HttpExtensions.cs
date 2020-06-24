using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace Agile4SMB.Client.Utils
{
    public static class HttpExtensions
    {
        public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
        {
            var content = new ObjectContent<T>(value, new JsonMediaTypeFormatter());
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri) { Content = content };

            return client.SendAsync(request);
        }
    }
}
