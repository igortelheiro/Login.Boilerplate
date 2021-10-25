using MediatR;
using MGR.Login.Application.Commands;
using MGR.Login.Application.Models;
using MGR.Login.Application.Services.Interfaces;
using MGR.Login.Infra.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MGR.Login.Application.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        #region Initialize
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenProviderService _tokenProvider;

        public LoginCommandHandler(UserManager<ApplicationUser> userManager,
                                   SignInManager<ApplicationUser> signInManager,
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

            string token = _tokenProvider.GenerateJwt(user);
            string refreshToken = string.Empty;
            string nomeUsuario = user.NomeCompleto;

            if(command.RememberMe == true)
            {
                refreshToken = await _tokenProvider.GenerateAndStoreRefreshTokenAsync(user);
            }
            
            return new LoginResult { Name = nomeUsuario, Token = token, RefreshToken = refreshToken };
        }


        private async Task<ApplicationUser> GetUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
                throw new ArgumentException($"Usuário não encontrado através do email {email}");

            return user;
        }


        private async Task ValidateCredentialsAsync(ApplicationUser user, LoginCommand command)
        {
            var credentialsValidation = await _signInManager
                .PasswordSignInAsync(user, command.Password, command.RememberMe,
                    lockoutOnFailure: false).ConfigureAwait(false);

            if (credentialsValidation.Succeeded == false)
            {
                var isEmailConfirmed = credentialsValidation.IsNotAllowed == false;
                var errorMessage = isEmailConfirmed ? "Senha inválida" : "Email não confirmado";
                throw new ArgumentException(errorMessage);
            }
        }
    }
}
