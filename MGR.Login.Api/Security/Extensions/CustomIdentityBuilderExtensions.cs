using System;
using MGR.Login.Domain;
using MGR.Login.Domain.Extensions;
using Microsoft.AspNetCore.Identity;

namespace MGR.Login.Api.Security.Extensions
{
    public static class CustomIdentityBuilderExtensions
    {
        public static IdentityBuilder AddCustomTokenProviders(this IdentityBuilder builder) =>
            builder.AddTokenProvider(typeof(RefreshTokenProvider<>), TokenPurpose.Refresh)
                   .AddTokenProvider(typeof(EmailConfirmationTokenProvider<>), TokenPurpose.EmailConfirmation)
                   .AddTokenProvider(typeof(ResetPasswordTokenProvider<>), TokenPurpose.ResetPassword);


        private static IdentityBuilder AddTokenProvider(this IdentityBuilder builder, Type tokenProviderType, TokenPurpose tokenPurpose)
        {
            var userType = builder.UserType;
            var tokenProvider = tokenProviderType.MakeGenericType(userType);
            return builder.AddTokenProvider(tokenPurpose.GetProviderName(), tokenProvider);
        }
    }
}
