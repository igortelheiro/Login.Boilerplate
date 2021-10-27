using MGR.Login.Api.Security.Base;
using MGR.Login.Api.Security.Options;
using MGR.Login.Domain;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MGR.Login.Api.Security
{
    public class EmailConfirmationTokenProvider<TUser> : BaseNumericTokenProvider<TUser>
        where TUser : IdentityUser
    {
        public EmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
                                              IOptions<EmailConfirmationTokenProviderOptions> options,
                                              ILogger<EmailConfirmationTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
            SetTokenPurpose(TokenPurpose.EmailConfirmation.ToString());
        }
    }
}
