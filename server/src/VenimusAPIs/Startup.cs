using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using VenimusAPIs.Registration;
using VenimusAPIs.Validation;

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

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "Roles",
                    RoleClaimType = "https://Venimus.YorkDevelopers.org/roles",
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Add the access_token as a claim, as we may actually need it
                        var accessToken = context.SecurityToken as JwtSecurityToken;
                        if (accessToken != null)
                        {
                            if (context.Principal.Identity is ClaimsIdentity identity)
                            {
                                identity.AddClaim(new Claim("access_token", accessToken.RawData));
                            }
                        }

                        return Task.CompletedTask;
                    },
                };
            });

            services.AddMvc(options =>
            {
                options.Filters.AddService<CheckGroupSecurityFilter>();
            }
            );

            services.AddScoped<CheckGroupSecurityFilter>();

            services.AddScoped<Mongo.GetFutureEventsQuery>();
            services.AddScoped<Mongo.EventStore>();
            services.AddScoped<Mongo.GroupStore>();
            services.AddScoped<Mongo.UserStore>();

            services.AddSingleton<Services.Auth0API>();

            services.AddControllers();

            services.AddSwagger();

            services.AddHealthChecks();

            services.AddHttpClient("Auth0", client =>
            {
                client.BaseAddress = new System.Uri($"https://{Configuration["Auth0:Domain"]}");
            });

            services.AddMassTransit();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBusControl busControl)
        {
            app.StartMassTransitBusIfAvailable(busControl);

            app.Use(async (ctx, next) =>
            {
                ctx.Response.Headers.Add("Content-Security-Policy", "default-src 'self';");
                await next();
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.AddLocalisation();

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
