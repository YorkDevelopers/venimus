using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Serilog;

namespace VenimusAPIs
{
    public static class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, false)
            .AddJsonFile(
                $"appsettings.{System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                true,
                false)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            System.Console.WriteLine(@"************************************************************************");
            System.Console.WriteLine(@"____    ____  _______ .__   __.  __  .___  ___.  __    __       _______.");
            System.Console.WriteLine(@"\   \  /   / |   ____||  \ |  | |  | |   \/   | |  |  |  |     /       |");
            System.Console.WriteLine(@" \   \/   /  |  |__   |   \|  | |  | |  \  /  | |  |  |  |    |   (----`");
            System.Console.WriteLine(@"  \      /   |   __|  |  . `  | |  | |  |\/|  | |  |  |  |     \   \    ");
            System.Console.WriteLine(@"   \    /    |  |____ |  |\   | |  | |  |  |  | |  `--'  | .----)   | ");
            System.Console.WriteLine(@"    \__/     |_______||__| \__| |__| |__|  |__|  \______/  |_______/    ");
            System.Console.WriteLine();
            System.Console.WriteLine(@"************************************************************************");
#pragma warning restore CA1303 // Do not pass literals as localized parameters

            IdentityModelEventSource.ShowPII = true;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            Log.Information("Application starting...");

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
        }
    }
}
