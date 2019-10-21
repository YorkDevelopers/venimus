using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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

        public Task<string> GetToken()
        {
            return Task.FromResult(CreateToken());
            /*
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
            */
        }

        private string CreateToken()
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(
              new RSAParameters()
              {
                  Modulus = FromBase64Url("w7Zdfmece8iaB0kiTY8pCtiBtzbptJmP28nSWwtdjRu0f2GFpajvWE4VhfJAjEsOcwYzay7XGN0b-X84BfC8hmCTOj2b2eHT7NsZegFPKRUQzJ9wW8ipn_aDJWMGDuB1XyqT1E7DYqjUCEOD1b4FLpy_xPn6oV_TYOfQ9fZdbE5HGxJUzekuGcOKqOQ8M7wfYHhHHLxGpQVgL0apWuP2gDDOdTtpuld4D2LK1MZK99s9gaSjRHE8JDb1Z4IGhEcEyzkxswVdPndUWzfvWBBWXWxtSUvQGBRkuy1BHOa4sP6FKjWEeeF7gm7UMs2Nm2QUgNZw6xvEDGaLk4KASdIxRQ"),
                  Exponent = FromBase64Url("AQAB"),
              });
            var key = new RsaSecurityKey(rsa);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "https://localhost:5001",
                Expires = DateTime.UtcNow.AddDays(7),
                Audience = "https://Venimus.YorkDevelopers.org",
                SigningCredentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.RsaSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0
                ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/")
                                  .Replace("-", "+");
            var s = Convert.FromBase64String(base64);
            return s;
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
