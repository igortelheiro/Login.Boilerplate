using Login.Application.Commands;
using Login.Application.Extensions;
using Login.Application.Models;
using Login.Application.Services.Interfaces;
using Login.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Login.Application.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        #region Initialize
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenProviderService _tokenProvider;
        private readonly IEmailBuilderService _emailBuilder;
        private readonly IServiceProvider _serviceProvider;

        public LoginCommandHandler(UserManager<ApplicationUser> userManager,
                                   SignInManager<ApplicationUser> signInManager,
                                   IEmailBuilderService emailBuilder,
                                   ITokenProviderService tokenProvider,
                                   IServiceProvider serviceProvider)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _emailBuilder = emailBuilder ?? throw new ArgumentNullException(nameof(emailBuilder));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
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


        private async Task<ApplicationUser> GetUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new ArgumentException($"Usuário não encontrado através do email {email}");

            return user;
        }


        private async Task ValidateCredentialsAsync(ApplicationUser user, LoginCommand command)
        {
            var validation = await _signInManager
                .PasswordSignInAsync(user, command.Password, command.RememberMe,
                    lockoutOnFailure: false).ConfigureAwait(false);
            if (validation.Succeeded)
                return;
            
            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (isEmailConfirmed)
                throw new ArgumentException("Senha inválida");

            await SendEmailConfirmationAsync(user);
            throw new ArgumentException("Verificação de email pendente. Um email de confirmação foi enviado para você");
        }


        private async Task SendEmailConfirmationAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var email = _emailBuilder.BuildAccontConfirmationEmail(user, token);
            await email.Send(_serviceProvider);
        }
    }
}
