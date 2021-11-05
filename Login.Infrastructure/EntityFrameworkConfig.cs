using System;
using System.Data.Common;
using Login.EntityFrameworkAdapter.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Login.EntityFrameworkAdapter
{
    public static class EntityFrameworkConfig
    {
        public static void ConfigureEntityFramework(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DbConnection")
                ?? throw new ArgumentException("ConnectionStrings.DbConnection não encontrado no appsettings");

            services.AddScoped<DbConnection>(_ => new SqlConnection(connectionString));

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        }
    }
}
