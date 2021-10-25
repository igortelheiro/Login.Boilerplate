using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MGR.Login.Application.Commands;
using MGR.Login.Application.Models;
using MGR.Login.Infra.Users;
using MGR.Login.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MGR.Login.Application.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
    {
        #region Initialize
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailBuilderService _emailBuilder;

        public RegisterCommandHandler(UserManager<ApplicationUser> userManager,
                                      IEmailBuilderService emailBuilder)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailBuilder = emailBuilder ?? throw new ArgumentNullException(nameof(emailBuilder));
        }
        #endregion


        public async Task<RegisterResult> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            var newUser = await CreateUserAsync(command);

            await SendAccountConfirmationEmail(newUser);

            return new RegisterResult { NewUserId = newUser.Id };
        }


        private async Task<ApplicationUser> CreateUserAsync(RegisterCommand command)
        {
            var newUser = new ApplicationUser
            {
                Email = command.Email,
                NomeCompleto = command.NomeCompleto,
                UserName = command.Email,
                PhoneNumber = command.PhoneNumber,
                CondominioId = command.CondominioId,
                Bloco = command.Bloco,
                NumeroApto = command.NumeroApto,
                EmailConfirmed = true //Remover após implementação da confirmação de email
            };

            var result = await _userManager.CreateAsync(newUser, command.Password).ConfigureAwait(false);
            if (result.Succeeded == false)
            {
                var error = result.Errors.FirstOrDefault()?.Description;
                throw new ArgumentException(error);
            }

            return newUser;
        }


        private async Task SendAccountConfirmationEmail(ApplicationUser user)
        {
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encryptedToken = EncryptToken(confirmationToken);
            var email = _emailBuilder.BuildAccontConfirmationEmail(user, encryptedToken);
            //TODO: Enviar email para um EmailService
        }


        private static string EncryptToken(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);
            var encodedToken = Base64UrlEncoder.Encode(bytes);
            return encodedToken;
        }
    }
}
