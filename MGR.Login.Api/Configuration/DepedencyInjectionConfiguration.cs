using MediatR;
using MGR.Login.Application.Commands;
using MGR.Login.Application.Services;
using MGR.Login.Application.Services.Interfaces;
using MGR.Login.Common;
using MGR.Login.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MGR.Login.Api.Configuration
{
    public static class DepedencyInjectionConfiguration
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailBuilderService, EmailBuilderService>();
            services.AddScoped<ITokenProviderService, TokenProviderService>();

            // Context
            services.ConfigureDbContext(configuration);

            // Security
            services.ConfigureSecurityOptions(configuration);

            // MediatR
            services.AddMediatR(typeof(LoginCommand));

            return services;
        }


        private static void ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString(Connections.LoginDb)));
        }
    }
}
