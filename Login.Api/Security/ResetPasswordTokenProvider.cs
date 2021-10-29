using Login.Api.Security.Base;
using Login.Api.Security.Options;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;

namespace Login.Api.Security
{
    public class ResetPasswordTokenProvider<TUser> : BaseTokenProvider<TUser>
        where TUser : IdentityUser
    {
        public ResetPasswordTokenProvider(IDataProtectionProvider dataProtectionProvider,
                                          IOptions<ResetPasswordTokenProviderOptions> options,
                                          ILogger<ResetPasswordTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
        }

        protected override Task<string> GenerateCustomToken()
        {
            var numericToken = new Random().Next(999999).ToString();
            return Task.FromResult(numericToken);
        }
    }
}
