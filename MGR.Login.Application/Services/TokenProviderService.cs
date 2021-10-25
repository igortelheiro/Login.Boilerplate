using MGR.Login.Application.Commands;
using MGR.Login.Application.Services.Interfaces;
using MGR.Login.Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MGR.Login.Infra.Users;

namespace MGR.Login.Application.Services
{
    public class TokenProviderService : ITokenProviderService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtConfiguration _jwtConfiguration;
        public TokenProviderService(UserManager<ApplicationUser> userManager, IOptions<JwtConfiguration> jwtOptions)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _jwtConfiguration = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        }


        public string GenerateJwt(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim("id", user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var jwt = new JwtSecurityToken(
                _jwtConfiguration.Issuer,
                _jwtConfiguration.Audience,
                claims,
                _jwtConfiguration.Expiry,
                signingCredentials: _jwtConfiguration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }


        public async Task<string> GenerateAndStoreRefreshTokenAsync(ApplicationUser user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, _jwtConfiguration.Issuer, nameof(RefreshTokenCommand.RefreshToken));
            var refreshToken = await _userManager.GenerateUserTokenAsync(user, _jwtConfiguration.Issuer, nameof(RefreshTokenCommand.RefreshToken));
            await _userManager.SetAuthenticationTokenAsync(user, _jwtConfiguration.Issuer, nameof(RefreshTokenCommand.RefreshToken), refreshToken);

            return refreshToken;
        }


        public async Task<string> RetrieveRefreshTokenAsync(ApplicationUser user) =>
            await _userManager.GetAuthenticationTokenAsync(user, _jwtConfiguration.Issuer, nameof(RefreshTokenCommand.RefreshToken));
    }
}
