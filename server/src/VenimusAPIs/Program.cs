using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace VenimusAPIs
{
    public class Program
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
            System.Console.WriteLine(@"************************************************************************");
            System.Console.WriteLine(@"____    ____  _______ .__   __.  __  .___  ___.  __    __       _______.");
            System.Console.WriteLine(@"\   \  /   / |   ____||  \ |  | |  | |   \/   | |  |  |  |     /       |");
            System.Console.WriteLine(@" \   \/   /  |  |__   |   \|  | |  | |  \  /  | |  |  |  |    |   (----`");
            System.Console.WriteLine(@"  \      /   |   __|  |  . `  | |  | |  |\/|  | |  |  |  |     \   \    ");
            System.Console.WriteLine(@"   \    /    |  |____ |  |\   | |  | |  |  |  | |  `--'  | .----)   | ");
            System.Console.WriteLine(@"    \__/     |_______||__| \__| |__| |__|  |__|  \______/  |_______/    ");
            System.Console.WriteLine();
            System.Console.WriteLine(@"************************************************************************");

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            Log.Information("Application starting...");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                });
    }
}
