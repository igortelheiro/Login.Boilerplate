using Login.Application.Commands;
using Login.Application.Models;
using Login.Application.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Login.Application.Extensions;

namespace Login.Application.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
    {
        #region Initialize
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailBuilderService _emailBuilder;
        private readonly IServiceProvider _serviceProvider;

        public RegisterCommandHandler(UserManager<IdentityUser> userManager,
                                      IEmailBuilderService emailBuilder,
                                      IServiceProvider serviceProvider)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailBuilder = emailBuilder ?? throw new ArgumentNullException(nameof(emailBuilder));
            _serviceProvider = serviceProvider;
        }
        #endregion


        public async Task<RegisterResult> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            var newUser = await CreateUserAsync(command);

            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var email = _emailBuilder.BuildAccontConfirmationEmail(newUser, confirmationToken);
            await email.Send(_serviceProvider);

            return new RegisterResult { NewUserId = newUser.Id };
        }


        private async Task<IdentityUser> CreateUserAsync(RegisterCommand command)
        {
            var newUser = new IdentityUser
            {
                Email = command.Email,
                UserName = command.Email,
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
    }
}
