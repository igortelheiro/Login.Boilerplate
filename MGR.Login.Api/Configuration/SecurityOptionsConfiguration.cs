using System;
using System.Text;
using MGR.Login.Common;
using MGR.Login.Infra.Context;
using MGR.Login.Infra.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace MGR.Login.Api.Configuration
{
    public static class SecurityOptionsConfiguration
    {
        public static void ConfigureSecurityOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureJwtOptions(configuration);
            services.ConfigureIdentityOptions(configuration);
        }


        private static void ConfigureJwtOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[Jwt.SecurityKey]));
            var issuer = configuration[Jwt.Issuer];
            var audience = configuration[Jwt.Audience];

            services.Configure<JwtConfiguration>(options =>
            {
                options.Issuer = issuer;
                options.Audience = audience;
                options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                options.Expiry = DateTime.Now.AddHours(int.Parse(configuration[Jwt.ExpiryInHours]));
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = securityKey
                    };
                });
        }


        private static void ConfigureIdentityOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            });

            services.AddDefaultIdentity<ApplicationUser>()
                .AddTokenProvider(configuration[Jwt.Issuer], typeof(DataProtectorTokenProvider<ApplicationUser>))
                .AddErrorDescriber<PortugueseIdentityErrorDescriber>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
        }
    }
}
