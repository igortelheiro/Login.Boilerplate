using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Login.Application.Commands;
using Login.Application.Extensions;
using Login.Application.Models;
using Login.Application.Services.Interfaces;
using MediatR;
using MGR.EventBus.Events;
using MGR.EventBus.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Login.Application.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
    {
        #region Initialize
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailBuilderService _emailBuilder;
        private readonly IEventBus _bus;

        public RegisterCommandHandler(UserManager<IdentityUser> userManager, IEmailBuilderService emailBuilder, IEventBus bus)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailBuilder = emailBuilder ?? throw new ArgumentNullException(nameof(emailBuilder));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }
        #endregion


        public async Task<RegisterResult> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            var newUser = await CreateUserAsync(command);

            var userCreated = new UserCreatedEvent
            {
                UserId = newUser.Id,
                Name = newUser.UserName,
                Email = newUser.Email
            };
            await _bus.Publish(userCreated);

            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var accountConfirmationEmailRequest = _emailBuilder.BuildAccontConfirmationEmail(newUser, confirmationToken);
            await accountConfirmationEmailRequest.Send(_bus);

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
