using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MockAuth
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

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
    }
}
