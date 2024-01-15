using System.Net.Http;
using System.Threading.Tasks;

namespace Gitea.API
{
    public static class HttpUtils
    {

        public static Task<HttpResponseMessage> PatchAsync(this HttpClient httpClient, string url,
            HttpContent content)
        {
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = content
            };
            return httpClient.SendAsync(request);
        }
    }
}