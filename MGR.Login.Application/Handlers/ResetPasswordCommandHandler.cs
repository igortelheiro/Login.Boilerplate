using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MGR.Login.Application.Commands;
using MGR.Login.Infra.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MGR.Login.Application.Handlers
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {
        #region Initilize
        private readonly UserManager<ApplicationUser> _userManager;
        public ResetPasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
        #endregion


        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await GetUserAsync(request.Email);

            ValidateNewPassword(user, request.NewPassword);

            await ResetPasswordAsync(user, request);
            return new Unit();
        }


        private async Task<ApplicationUser> GetUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
                throw new ArgumentException($"Usuário não encontrado através do email {email}");

            return user;
        }


        private void ValidateNewPassword(ApplicationUser user, string newPassword)
        {
            var passwordValidationError = string.Empty;
            var isPasswordValid = _userManager.PasswordValidators.All(v =>
            {
                var validation = v.ValidateAsync(_userManager, user, newPassword).Result;
                if (validation.Succeeded == false)
                    passwordValidationError = validation.Errors.FirstOrDefault()?.Description;
                
                return validation.Succeeded;
            });

            if (isPasswordValid == false)
                throw new ArgumentException(passwordValidationError);
        }


        private async Task ResetPasswordAsync(ApplicationUser user, ResetPasswordCommand request)
        {
            var decodedToken = Base64UrlEncoder.Decode(request.Token);
            var passwordReset = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword).ConfigureAwait(false);
            if (passwordReset.Succeeded == false)
            {
                var error = passwordReset.Errors.FirstOrDefault()?.Description;
                throw new ArgumentException(error);
            }
        }
    }
}
