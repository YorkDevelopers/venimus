using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using VenimusAPIs.Tests.Extensions;

namespace VenimusAPIs.Tests.Infrastucture
{
    public class APIClient
    {
        private readonly HttpClient _client;

        public APIClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return await _client.DeleteAsync(requestUri);
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return await _client.GetAsync(requestUri);
        }

        public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T data)
        {
            return await _client.PutAsJsonAsync(requestUri, data);
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T data)
        {
            return await _client.PostAsJsonAsync(requestUri, data);
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri)
        {
            return await _client.PostAsync(requestUri, null);
        }

        public async Task<HttpResponseMessage> PatchAsync(string requestUri)
        {
            return await _client.PatchAsync(requestUri, null);
        }

        internal async Task<HttpResponseMessage> PatchAsJsonAsync<T>(string requestUri, T data)
        {
            return await _client.PatchAsJsonAsync(requestUri, data);
        }

        public async Task<T> GetAsJsonAsync<T>(string requestUri)
        {
            var result = await _client.GetAsync(requestUri);
            result.EnsureSuccessStatusCode();

            var json = await result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json);
        }

        internal void ClearBearerToken()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
        }

        internal void SetDigest(string digest)
        {
            _client.DefaultRequestHeaders.Remove("Digest");
            _client.DefaultRequestHeaders.Add("Digest", digest);
        }

        internal void SetBearerToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        internal void SetHeader(string name, string value)
        {
            _client.DefaultRequestHeaders.Remove(name);
            _client.DefaultRequestHeaders.Add(name, value);
        }

        internal void ClearHeader(string name)
        {
            _client.DefaultRequestHeaders.Remove(name);
        }

        internal void SetCulture(string culture)
        {
            _client.DefaultRequestHeaders.AcceptLanguage.Clear();
            _client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(culture));
        }

        internal void ResetCulture()
        {
            _client.DefaultRequestHeaders.AcceptLanguage.Clear();
        }
    }
}
