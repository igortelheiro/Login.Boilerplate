using Login.Application.Commands;
using Login.Application.Services;
using Login.Application.Services.Interfaces;
using Login.Infrastructure.Context;
using MediatR;
using MGR.RabbitMQEventBus.Configuration;
using Microsoft.EntityFrameworkCore;
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

            services.ConfigureRabbitMQEventBus();

            services.ConfigureDbContext(configuration);

            services.ConfigureSecurityOptions(configuration);

            services.AddMediatR(typeof(LoginCommand));

            return services;
        }


        private static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("LoginDb")));
        }


        private static void ConfigureSecurityOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureIdentityOptions();
            services.ConfigureTokenProvidersOptions();
            services.ConfigureJwtOptions(configuration);
        }
    }
}
