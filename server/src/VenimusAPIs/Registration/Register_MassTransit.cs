using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using VenimusAPIs.ServiceBusMessages;

namespace VenimusAPIs.Registration
{
    public static class Register_MassTransit
    {
        public static void AddMassTransit(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<GroupChangedConsumer>();

                x.AddBus(provider => SetupBusControl(provider));
            });
        }

        private static IBusControl SetupBusControl(IServiceProvider provider)
        {
            return Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ReceiveEndpoint("venimus-events", ep =>
                {
                    ep.ConfigureConsumer<GroupChangedConsumer>(provider);
                });
            });
        }

        public static void StartMassTransitBusIfAvailable(this IApplicationBuilder applicationBuilder, IBusControl busControl)
        {
            try
            {
                Log.Logger.Information("Starting MassTransit service bus");
                busControl.Start();
            }
            catch (Exception ex)
            {
                var message = "Failed to start mass transit - " + ex.Message;
                Log.Logger.Error(message);
                throw;
            }
        }
    }
}
