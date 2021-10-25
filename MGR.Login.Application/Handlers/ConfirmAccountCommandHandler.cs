using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MGR.Login.Application.Commands;
using MGR.Login.Infra.Users;
using Microsoft.AspNetCore.Identity;

namespace MGR.Login.Application.Handlers
{
    public class ConfirmAccountCommandHandler : IRequestHandler<ConfirmAccountCommand>
    {
        #region Initialize
        private readonly UserManager<ApplicationUser> _userManager;
        public ConfirmAccountCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
        #endregion


        public async Task<Unit> Handle(ConfirmAccountCommand command, CancellationToken cancellationToken)
        {
            var user = await GetUserAsync(command.Email);

            await CheckEmailAlreadyConfirmedAsync(user);
            
            await ConfirmEmailAsync(user, command.Token).ConfigureAwait(false);

            return new Unit();
        }


        private async Task<ApplicationUser> GetUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
                throw new ArgumentException($"Usuário não encontrado através do email {email}");

            return user;
        }


        private async Task CheckEmailAlreadyConfirmedAsync(ApplicationUser user)
        {
            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user).ConfigureAwait(false);
            if (isEmailConfirmed)
                throw new ArgumentException("Email já confirmado");
        }


        private async Task ConfirmEmailAsync(ApplicationUser user, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token de confirmação não informado");

            var confirmation = await _userManager.ConfirmEmailAsync(user, token).ConfigureAwait(false);
            if (confirmation.Succeeded == false)
            {
                var confirmationError = confirmation.Errors.FirstOrDefault()?.Description;
                throw new ArgumentException(confirmationError);
            }
        }
    }
}
