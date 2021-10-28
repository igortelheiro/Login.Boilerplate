using MediatR;
using MGR.Login.Application.Commands;
using MGR.Login.Application.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MGR.Login.Application.Services.Interfaces;
using MGR.Login.Domain;

namespace MGR.Login.Application.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResult>
    {
        #region Initialize
        private readonly ITokenProviderService _tokenProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RefreshTokenCommandHandler(UserManager<ApplicationUser> userManager,
                                   SignInManager<ApplicationUser> signInManager,
                                   ITokenProviderService tokenProvider)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }
        #endregion


        public async Task<LoginResult> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var user = await GetUserAsync(command.Email);

            await ValidateRefreshTokenAsync(user, command);

            var token = await _tokenProvider.GenerateJwt(user);
            var refreshToken = await _tokenProvider.GenerateAndStoreTokenAsync(user, TokenPurpose.Refresh);

            return new LoginResult { Token = token, RefreshToken = refreshToken };
        }


        private async Task<ApplicationUser> GetUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
                throw new ArgumentException($"Usuário não encontrado através do email {email}");

            return user;
        }


        private async Task ValidateRefreshTokenAsync(ApplicationUser user, RefreshTokenCommand command)
        {
            var validToken = await _tokenProvider.ValidateTokenAsync(user, TokenPurpose.Refresh, command.RefreshToken);
            if (validToken == false)
                throw new ArgumentException("Token inválido ou expirado");
        }
    }
}
