using Login.Api.Security.Base;
using Login.Api.Security.Options;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Login.Api.Security
{
    public class EmailConfirmationTokenProvider<TUser> : BaseNumericTokenProvider<TUser>
        where TUser : IdentityUser
    {
        public EmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
                                              IOptions<EmailConfirmationTokenProviderOptions> options,
                                              ILogger<EmailConfirmationTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
        }
    }
}
