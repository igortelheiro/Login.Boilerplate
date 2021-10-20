using MediatR;
using MGR.Login.Application.Commands;
using MGR.Login.Application.Models;
using MGR.Login.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MGR.Login.Application.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        #region Initialize
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ITokenProviderService _tokenProvider;

        public LoginCommandHandler(UserManager<IdentityUser> userManager,
                                   SignInManager<IdentityUser> signInManager,
                                   ITokenProviderService tokenProvider)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }
        #endregion


        public async Task<LoginResult> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await GetUserAsync(command.Email);

            await ValidateCredentialsAsync(user, command);

            var token = _tokenProvider.GenerateJwt(user);
            var refreshToken = command.RememberMe
                ? await _tokenProvider.GenerateAndStoreRefreshTokenAsync(user)
                : null;

            return new LoginResult { Token = token, RefreshToken = refreshToken };
        }


        private async Task<IdentityUser> GetUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
                throw new ArgumentException($"Usuário não encontrado através do email {email}");

            return user;
        }


        private async Task ValidateCredentialsAsync(IdentityUser user, LoginCommand command)
        {
            var credentialsValidation = await _signInManager
                .PasswordSignInAsync(user, command.Password, command.RememberMe,
                    lockoutOnFailure: false).ConfigureAwait(false);

            if (credentialsValidation.Succeeded == false)
            {
                var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
                var errorMessage = isEmailConfirmed
                    ? "Senha inválida"
                    : "Email não confirmado";

                throw new ArgumentException(errorMessage);
            }
        }
    }
}
