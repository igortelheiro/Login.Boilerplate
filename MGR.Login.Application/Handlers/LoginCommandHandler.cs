using MediatR;
using MGR.Login.Application.Commands;
using MGR.Login.Application.Models;
using MGR.Login.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using MGR.Login.Application.Extensions;
using MGR.Login.Domain;

namespace MGR.Login.Application.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        #region Initialize
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailBuilderService _emailBuilder;
        private readonly ITokenProviderService _tokenProvider;

        public LoginCommandHandler(UserManager<IdentityUser> userManager,
                                   SignInManager<IdentityUser> signInManager,
                                   ITokenProviderService tokenProvider,
                                   IEmailBuilderService emailBuilder)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _emailBuilder = emailBuilder ?? throw new ArgumentNullException(nameof(emailBuilder));
        }
        #endregion


        public async Task<LoginResult> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await GetUserAsync(command.Email);

            await ValidateCredentialsAsync(user, command);

            var token = await _tokenProvider.GenerateJwt(user);
            var refreshToken = command.RememberMe
                ? await _tokenProvider.GenerateAndStoreTokenAsync(user, TokenPurpose.Refresh)
                : null;

            return new LoginResult { Token = token, RefreshToken = refreshToken };
        }


        private async Task<IdentityUser> GetUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new ArgumentException($"Usuário não encontrado através do email {email}");

            return user;
        }


        private async Task ValidateCredentialsAsync(IdentityUser user, LoginCommand command)
        {
            var validation = await _signInManager
                .PasswordSignInAsync(user, command.Password, command.RememberMe, lockoutOnFailure: false);

            if (validation.Succeeded)
                return;
            
            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (isEmailConfirmed)
                throw new ArgumentException("Senha inválida");

            await SendEmailConfirmationAsync(user);
            throw new ArgumentException("Verificação de email pendente. Um email de confirmação foi enviado para você");
        }


        private async Task SendEmailConfirmationAsync(IdentityUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var email = _emailBuilder.BuildAccontConfirmationEmail(user, token);
            await email.Send();
        }
    }
}
