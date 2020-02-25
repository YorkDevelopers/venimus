using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using VenimusAPIs.ServiceBus;

namespace VenimusAPIs.Registration
{
    public static class RegisterMassTransit
    {
        public static void AddMassTransit(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumersFromNamespaceContaining<UserChangedConsumer>();

                x.AddMediator((provider, cfg) =>
                {
                    cfg.ConfigureConsumers(provider);
                });
            });
        }
    }
}
