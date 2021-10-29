using Login.Api.Security.Base;
using Login.Api.Security.Options;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Login.Api.Security
{
    public class ResetPasswordTokenProvider<TUser> : BaseNumericTokenProvider<TUser>
        where TUser : IdentityUser
    {
        public ResetPasswordTokenProvider(IDataProtectionProvider dataProtectionProvider,
                                          IOptions<ResetPasswordTokenProviderOptions> options,
                                          ILogger<ResetPasswordTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
        }
    }
}
