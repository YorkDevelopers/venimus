using System.IO;
using AutoMapper;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using VenimusAPIs.Registration;

namespace VenimusAPIs
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(GetType().Assembly);

            services.Configure<Settings.MongoDBSettings>(
                options => Configuration.GetSection("MongoDB").Bind(options));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = $"https://{Configuration["Auth0:Domain"]}/";
                options.Audience = Configuration["Auth0:Audience"];
            });

            services.AddSingleton<Services.Mongo>();

            services.AddControllers();

            services.AddSwagger();

            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Use(async (ctx, next) =>
            {
                ctx.Response.Headers.Add("Content-Security-Policy", "default-src 'self';");
                await next();
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (env.IsEnvironment("Testing") || env.IsDevelopment())
            {
                app.Use(async (ctx, next) =>
                {
                    if (ctx.Request.Path.Value == @"/.well-known/openid-configuration")
                    {
                        var contents = await File.ReadAllTextAsync(@"MockOpenID\openid-configuration.txt");
                        await ctx.Response.WriteAsync(contents);
                    }
                    else if (ctx.Request.Path.Value == @"/.well-known/jwks.json")
                    {
                        var contents = await File.ReadAllTextAsync(@"MockOpenID\jwks.json");
                        await ctx.Response.WriteAsync(contents);
                    }
                    else
                    {
                        await next();
                    }
                });
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });
        }
    }
}
