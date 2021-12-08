using IntegrationEventLogEF.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Login.EventBusAdapter
{
    public static class EventBusConfig
    {
        public static void ConfigureEventBus(this IServiceCollection services, IConfiguration configuration) =>
            services.ConfigureIntegrationEventLog(configuration);
    }
}
