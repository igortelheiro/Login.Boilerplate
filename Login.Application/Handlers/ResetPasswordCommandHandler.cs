using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Login.Application.Commands;
using Login.Application.Services.Interfaces;
using Login.Domain;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Login.Application.Handlers
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {
        #region Initilize
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenProviderService _tokenProvider;
        public ResetPasswordCommandHandler(UserManager<IdentityUser> userManager, ITokenProviderService tokenProvider)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }
        #endregion


        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await GetUserAsync(request.Email);

            ValidateNewPassword(user, request.NewPassword);
            await ResetPasswordAsync(user, request);

            return new Unit();
        }


        private async Task<IdentityUser> GetUserAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new ArgumentException($"Usuário não encontrado através do email {email}");

            return user;
        }


        private void ValidateNewPassword(IdentityUser user, string newPassword)
        {
            _userManager.PasswordValidators.All(v =>
            {
                var validation = v.ValidateAsync(_userManager, user, newPassword).Result;
                if (validation.Succeeded) return true;
                
                var passwordValidationError = validation.Errors.FirstOrDefault()?.Description;
                throw new ArgumentException(passwordValidationError);
            });
        }


        private async Task ResetPasswordAsync(IdentityUser user, ResetPasswordCommand request)
        {
            var passwordReset = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (passwordReset.Succeeded == false)
            {
                var error = passwordReset.Errors.FirstOrDefault()?.Description;
                throw new ArgumentException(error);
            }
        }
    }
}
