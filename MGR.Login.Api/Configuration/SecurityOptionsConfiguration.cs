using MGR.Login.Common;
using MGR.Login.Infra.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using MGR.Login.Api.Security.Extensions;
using MGR.Login.Api.Security.Options;
using MGR.Login.Domain;
using MGR.Login.Domain.Extensions;

namespace MGR.Login.Api.Configuration
{
    public static class SecurityOptionsConfiguration
    {
        public static void ConfigureIdentityOptions(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            });

            services.AddDefaultIdentity<IdentityUser>()
                .AddErrorDescriber<PortugueseIdentityErrorDescriber>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddCustomTokenProviders();
        }


        public static void ConfigureTokenProvidersOptions(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Tokens.PasswordResetTokenProvider = TokenPurpose.ResetPassword.GetProviderName();
                options.Tokens.EmailConfirmationTokenProvider = TokenPurpose.EmailConfirmation.GetProviderName();
            });

            services.Configure<RefreshTokenProviderOptions>(options =>
            {
                options.Name = TokenPurpose.Refresh.GetProviderName();
                options.TokenLifespan = TimeSpan.FromHours(2);
            });

            services.Configure<EmailConfirmationTokenProviderOptions>(options =>
            {
                options.Name = TokenPurpose.EmailConfirmation.GetProviderName();
                options.TokenLifespan = TimeSpan.FromMinutes(15);
            });

            services.Configure<ResetPasswordTokenProviderOptions>(options =>
            {
                options.Name = TokenPurpose.ResetPassword.GetProviderName();
                options.TokenLifespan = TimeSpan.FromMinutes(15);
            });
        }


        public static void ConfigureJwtOptions(this IServiceCollection services, IConfiguration configuration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[Jwt.SecurityKey]));
            var issuer = configuration[Jwt.Issuer];
            var audience = configuration[Jwt.Audience];
            var expiryInHours = int.Parse(configuration[Jwt.ExpiryInHours]);

            services.Configure<JwtConfiguration>(options =>
            {
                options.Issuer = issuer;
                options.Audience = audience;
                options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                options.Expiry = DateTime.Now.AddHours(expiryInHours);
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
    }
}
