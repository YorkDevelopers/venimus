using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace VenimusAPIs.Extensions
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, Uri url, T data)
        {
            var dataAsString = Newtonsoft.Json.JsonConvert.SerializeObject(data);
#pragma warning disable CA2000 // Dispose objects before losing scope
            var content = new StringContent(dataAsString);
#pragma warning restore CA2000 // Dispose objects before losing scope
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PostAsync(url, content);
        }
    }
}
