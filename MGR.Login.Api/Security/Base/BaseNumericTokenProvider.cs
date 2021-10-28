﻿using System;
using System.IO;
using System.Threading.Tasks;
using MGR.Login.Api.Security.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MGR.Login.Api.Security.Base
{
    public abstract class BaseNumericTokenProvider<TUser> : DataProtectorTokenProvider<TUser>
        where TUser : IdentityUser
    {
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
            var numericToken = new Random().Next(999999).ToString();

            var ms = new MemoryStream();
            var userId = await manager.GetUserIdAsync(user);
            await using (var writer = ms.CreateWriter())
            {
                writer.Write(DateTimeOffset.UtcNow.UtcTicks);
                writer.Write(userId);
                writer.Write(purpose ?? "");
                writer.Write(numericToken);
                string stamp = null;
                if (manager.SupportsUserSecurityStamp)
                {
                    stamp = await manager.GetSecurityStampAsync(user);
                }
                writer.Write(stamp ?? "");
            }
            var protectedBytes = Protector.Protect(ms.ToArray());
            var encodedToken = Convert.ToBase64String(protectedBytes);

            await manager.SetAuthenticationTokenAsync(user, Options.Name, purpose, encodedToken);
            
            return numericToken;
        }


        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            try
            {
                var storedToken = await manager.GetAuthenticationTokenAsync(user, Options.Name, purpose);
                await manager.RemoveAuthenticationTokenAsync(user, Options.Name, purpose);

                var unprotectedData = Protector.Unprotect(Convert.FromBase64String(storedToken));
                var ms = new MemoryStream(unprotectedData);
                using (var reader = ms.CreateReader())
                {
                    var creationTime = new DateTimeOffset(reader.ReadInt64(), TimeSpan.Zero);
                    var expirationTime = creationTime + Options.TokenLifespan;
                    if (expirationTime < DateTimeOffset.UtcNow)
                    {
                        return false;
                    }

                    var userId = reader.ReadString();
                    var actualUserId = await manager.GetUserIdAsync(user);
                    if (userId != actualUserId)
                    {
                        return false;
                    }

                    var purp = reader.ReadString();
                    if (!string.Equals(purp, purpose))
                    {
                        return false;
                    }

                    var numericToken = reader.ReadString();
                    if (!string.Equals(numericToken, token))
                    {
                        return false;
                    }

                    var stamp = reader.ReadString();
                    if (reader.PeekChar() != -1)
                    {
                        return false;
                    }

                    if (manager.SupportsUserSecurityStamp)
                    {
                        var isEqualsSecurityStamp = stamp == await manager.GetSecurityStampAsync(user);
                        if (!isEqualsSecurityStamp)
                        {
                        }

                        return isEqualsSecurityStamp;
                    }


                    var stampIsEmpty = stamp == "";
                    if (!stampIsEmpty)
                    {
                    }

                    return stampIsEmpty;
                }
            }
            catch
            {
                // Do not leak exception
            }

            return false;
        }
    }
}
