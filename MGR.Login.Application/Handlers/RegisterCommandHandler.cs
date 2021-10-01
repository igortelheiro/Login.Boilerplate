using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MGR.Login.Application.Commands;
using MGR.Login.Application.Models;
using MGR.Login.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MGR.Login.Application.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
    {
        #region Initialize
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailBuilderService _emailBuilder;

        public RegisterCommandHandler(UserManager<IdentityUser> userManager,
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


        private async Task<IdentityUser> CreateUserAsync(RegisterCommand command)
        {
            var newUser = new IdentityUser
            {
                Email = command.Email,
                UserName = command.UserName,
                PhoneNumber = command.PhoneNumber
            };

            var result = await _userManager.CreateAsync(newUser, command.Password).ConfigureAwait(false);
            if (result.Succeeded == false)
            {
                var error = result.Errors.FirstOrDefault()?.Description;
                throw new ArgumentException(error);
            }

            return newUser;
        }


        private async Task SendAccountConfirmationEmail(IdentityUser user)
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
