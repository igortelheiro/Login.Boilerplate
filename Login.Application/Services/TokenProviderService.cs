using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Login.Application.Models;
using Login.Application.Services.Interfaces;
using Login.Domain;
using Login.Domain.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Login.Application.Services
{
    public class TokenProviderService : ITokenProviderService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfiguration _jwtConfiguration;
        public TokenProviderService(UserManager<IdentityUser> userManager, IOptions<JwtConfiguration> jwtOptions)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _jwtConfiguration = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        }


        public async Task<string> GenerateJwt(IdentityUser user)
        {
            var defaultClaims = new[]
            {
                new Claim("id", user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var userClaims = await _userManager.GetClaimsAsync(user);
            var claims = defaultClaims.Concat(userClaims);
            
            var jwt = new JwtSecurityToken(
                _jwtConfiguration.Issuer,
                _jwtConfiguration.Audience,
                claims,
                _jwtConfiguration.Expiry,
                signingCredentials: _jwtConfiguration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }


        public async Task<string> GenerateAndStoreTokenAsync(IdentityUser user, TokenPurpose tokenPurpose)
        {
            var tokenName = tokenPurpose.GetName();
            var tokenProvider = tokenPurpose.GetProviderName();

            await _userManager.RemoveAuthenticationTokenAsync(user, tokenProvider, tokenName);
            var token = await _userManager.GenerateUserTokenAsync(user, tokenProvider, tokenName);
            await _userManager.SetAuthenticationTokenAsync(user, tokenProvider, tokenName, token);
            
            return token;
        }


        public async Task<bool> ValidateTokenAsync(IdentityUser user, TokenPurpose tokenPurpose, string token)
        {
            var tokenName = tokenPurpose.GetName();
            var tokenProvider = tokenPurpose.GetProviderName();

            var valid = await _userManager.VerifyUserTokenAsync(user, tokenProvider, tokenName, token);
            await _userManager.RemoveAuthenticationTokenAsync(user, tokenProvider, tokenName);

            return valid;
        }
    }
}
