using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VenimusAPIs.Tests.Extensions;
using VenimusAPIs.Tests.Infrastucture;

namespace VenimusAPIs.Tests
{
    public abstract class BaseTest
    {
        private readonly WebApplicationFactory<Startup> _factory;

        private readonly WebApplicationFactory<Startup> _fixture;

        public BaseTest()
        {
            void ConfigureWebHostBuilder(IWebHostBuilder builder)
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureTestServices(services =>
                {
                });
            }

            _fixture = new WebApplicationFactory<Startup>();
            _factory = _fixture.Factories.FirstOrDefault() ?? _fixture.WithWebHostBuilder(ConfigureWebHostBuilder);
            _client = _factory.CreateClient();
        }

        public T GetService<T>() => _factory.Server.Host.Services.GetRequiredService<T>();

        public APIClient APIClient => new APIClient(_client);

        private readonly HttpClient _client;

        protected async Task<string> GetToken()
        {
            var builder = new ConfigurationBuilder()
                          .AddUserSecrets<BaseTest>();

            var configuration = builder.Build();

            var client_id = configuration["Auth0:client_id"];
            var client_secret = configuration["Auth0:client_secret"];

            var client = new HttpClient();
            client.BaseAddress = new System.Uri("https://dev-btvhsvy1.eu.auth0.com");

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", client_id),
                new KeyValuePair<string, string>("client_secret", client_secret),
                new KeyValuePair<string, string>("audience", "https://Venimus.YorkDevelopers.org"),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
            });

            var response = await client.PostAsync("/oauth/token", formContent);

            var details = await response.Content.ReadAsJsonAsync<AuthOResponse>();

            return details.AccessToken;
        }

        private class AuthOResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
        }
    }
}
