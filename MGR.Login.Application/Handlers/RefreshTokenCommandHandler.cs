using MediatR;
using MGR.Login.Application.Commands;
using MGR.Login.Application.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MGR.Login.Application.Services.Interfaces;

namespace MGR.Login.Application.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResult>
    {
        #region Initialize
        private readonly ITokenProviderService _tokenProvider;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public RefreshTokenCommandHandler(UserManager<IdentityUser> userManager,
                                   SignInManager<IdentityUser> signInManager,
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

            var token = _tokenProvider.GenerateJwt(user);
            var refreshToken = await _tokenProvider.GenerateAndStoreRefreshTokenAsync(user);

            return new LoginResult { Token = token, RefreshToken = refreshToken };
        }


        private async Task<IdentityUser> GetUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
                throw new ArgumentException($"Usuário não encontrado através do email {email}");

            return user;
        }


        private async Task ValidateRefreshTokenAsync(IdentityUser user, RefreshTokenCommand command)
        {
            var storedToken = await _tokenProvider.RetrieveRefreshTokenAsync(user);

            if (storedToken != command.RefreshToken)
                throw new ArgumentException("O RefreshToken fornecido é inválido");
        }
    }
}
