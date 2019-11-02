using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
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
            void ConfigureWebHostBuilder(IWebHostBuilder builder)
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(sp => MockAuth0);

                    services.AddHttpClient("Auth0")
                        .AddHttpMessageHandler<MockAuth0>();
                });
            }

            var builder = new ConfigurationBuilder()
                          .AddUserSecrets<Fixture>()
                          .SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json", false, false)
                          .AddJsonFile($"appsettings.Testing.json", true, false)
                          .AddEnvironmentVariables();
            _configuration = builder.Build();

            MockAuth0 = new MockAuth0();

            _fixture = new WebApplicationFactory<Startup>();
            _factory = _fixture.Factories.FirstOrDefault() ?? _fixture.WithWebHostBuilder(ConfigureWebHostBuilder);
            _client = _factory.CreateClient();
        }

        public T GetService<T>() => _factory.Server.Host.Services.GetRequiredService<T>();

        public APIClient APIClient => new APIClient(_client);

        public MockAuth0 MockAuth0 { get; }

        private readonly HttpClient _client;

        public string GetTokenForNewUser(string uniqueID)
        {
            return CreateMockToken(uniqueID, string.Empty);
        }

        public async Task<string> GetTokenForNormalUser(string uniqueID = "")
        {
            IdentityModelEventSource.ShowPII = true;

            // var token = await CreateAuth0Token();
            var token = CreateMockToken(uniqueID, string.Empty);
            return await Task.FromResult(token);
        }

        public async Task<string> GetTokenForSystemAdministrator()
        {
            IdentityModelEventSource.ShowPII = true;

            // var token = await CreateAuth0Token();
            var token = CreateMockToken(string.Empty, "SystemAdministrator");
            return await Task.FromResult(token);
        }

        private async Task<string> CreateAuth0Token()
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

        private string CreateMockToken(string uniqueID, string role)
        {
            using var rsa = RSA.Create(2048);
            RSAFromXmlFile(rsa, @"MockOpenId/private.xml");

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = "https://venimus-mockauth.azurewebsites.net/",
                Expires = DateTime.UtcNow.AddDays(7),
                Audience = "https://Venimus.YorkDevelopers.org",
                SigningCredentials = new SigningCredentials(
                    new RsaSecurityKey(rsa),
                    SecurityAlgorithms.RsaSha256),
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, uniqueID),
                    new Claim("https://Venimus.YorkDevelopers.org/roles", role),
                }),
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

        private void RSAFromXmlFile(RSA rsa, string filename)
        {
            RSAParameters parameters = new RSAParameters();

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = string.IsNullOrEmpty(node.InnerText) ? null : FromBase64Url(node.InnerText); break;
                        case "Exponent": parameters.Exponent = string.IsNullOrEmpty(node.InnerText) ? null : FromBase64Url(node.InnerText); break;
                        case "P": parameters.P = string.IsNullOrEmpty(node.InnerText) ? null : FromBase64Url(node.InnerText); break;
                        case "Q": parameters.Q = string.IsNullOrEmpty(node.InnerText) ? null : FromBase64Url(node.InnerText); break;
                        case "DP": parameters.DP = string.IsNullOrEmpty(node.InnerText) ? null : FromBase64Url(node.InnerText); break;
                        case "DQ": parameters.DQ = string.IsNullOrEmpty(node.InnerText) ? null : FromBase64Url(node.InnerText); break;
                        case "InverseQ": parameters.InverseQ = string.IsNullOrEmpty(node.InnerText) ? null : FromBase64Url(node.InnerText); break;
                        case "D": parameters.D = string.IsNullOrEmpty(node.InnerText) ? null : FromBase64Url(node.InnerText); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key.");
            }

            rsa.ImportParameters(parameters);
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
