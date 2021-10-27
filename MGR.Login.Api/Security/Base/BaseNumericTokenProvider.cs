using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MGR.Login.Api.Security.Base
{
    public abstract class BaseNumericTokenProvider<TUser> : DataProtectorTokenProvider<TUser>
        where TUser : IdentityUser
    {
        private string _tokenPurpose = string.Empty;

        protected BaseNumericTokenProvider(IDataProtectionProvider dataProtectionProvider,
                                           IOptions<BaseTokenProviderOptions> options,
                                           ILogger<DataProtectorTokenProvider<TUser>> logger)
            : base(dataProtectionProvider, options, logger)
        {
        }


        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user) =>
            Task.FromResult(false);


        public override async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            var token = new Random().Next(999999).ToString();

            await manager.SetAuthenticationTokenAsync(user, Options.Name, SelectTokenPurpose(purpose), token);
            
            return token;
        }


        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            var storedToken = await manager.GetAuthenticationTokenAsync(user, Options.Name, SelectTokenPurpose(purpose));
            var validToken = storedToken == token;
            
            await manager.RemoveAuthenticationTokenAsync(user, Options.Name, SelectTokenPurpose(purpose));
            
            return validToken;
        }


        protected void SetTokenPurpose(string purpose) => _tokenPurpose = purpose;
        protected string SelectTokenPurpose(string purpose) => _tokenPurpose ?? purpose;
    }
}
