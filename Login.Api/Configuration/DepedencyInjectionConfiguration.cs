using Login.Application.Commands;
using Login.Application.Services;
using Login.Application.Services.Interfaces;
using Login.EntityFrameworkAdapter;
using Login.EventBusAdapter;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Login.Api.Configuration
{
    public static class DepedencyInjectionConfiguration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailBuilderService, EmailBuilderService>();
            services.AddScoped<ITokenProviderService, TokenProviderService>();

            services.ConfigureEventBus();

            services.ConfigureEntityFramework(configuration);

            services.ConfigureSecurityOptions(configuration);

            services.AddMediatR(typeof(LoginCommand));

            return services;
        }


        private static void ConfigureSecurityOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureIdentityOptions();
            services.ConfigureTokenProvidersOptions();
            services.ConfigureJwtOptions(configuration);
        }
    }
}
