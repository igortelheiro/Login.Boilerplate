using Login.Api.Security.Options;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Login.Api.Security
{
    public class RefreshTokenProvider<TUser> : DataProtectorTokenProvider<TUser>
        where TUser : IdentityUser
    {
        public RefreshTokenProvider(IDataProtectionProvider dataProtectionProvider,
                                    IOptions<RefreshTokenProviderOptions> options,
                                    ILogger<RefreshTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
        }
    }
}
