using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
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
    }
}
