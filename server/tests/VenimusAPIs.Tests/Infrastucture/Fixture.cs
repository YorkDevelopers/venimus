using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using VenimusAPIs.Tests.Extensions;
using VenimusAPIs.Tests.Infrastucture;

namespace VenimusAPIs.Tests
{
    public class Fixture : WebApplicationFactory<Startup>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly IConfigurationRoot _configuration;
        private readonly WebApplicationFactory<Startup> _fixture;

        public Fixture()
        {
            static void ConfigureWebHostBuilder(IWebHostBuilder builder)
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureTestServices(services =>
                {
                });
            }

            var builder = new ConfigurationBuilder()
                          .AddUserSecrets<Fixture>()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json", false, false)
                          .AddJsonFile($"appsettings.Testing.json", true, false)
                          .AddEnvironmentVariables();
            _configuration = builder.Build();

            _fixture = new WebApplicationFactory<Startup>();
            _factory = _fixture.Factories.FirstOrDefault() ?? _fixture.WithWebHostBuilder(ConfigureWebHostBuilder);
            _client = _factory.CreateClient();
        }

        public T GetService<T>() => _factory.Server.Host.Services.GetRequiredService<T>();

        public APIClient APIClient => new APIClient(_client);

        private readonly HttpClient _client;

        public async Task<string> GetToken()
        {
            var client_id = _configuration["Auth0:client_id"];
            var client_secret = _configuration["Auth0:client_secret"];

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

        public IMongoDatabase MongoDatabase()
        {
            var connectionString = _configuration["MongoDB:connectionString"];
            var databaseName = _configuration["MongoDB:databaseName"];

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            return database;
        }
    }
}
