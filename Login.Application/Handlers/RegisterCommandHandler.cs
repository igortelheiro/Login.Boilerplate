using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Login.Application.Commands;
using Login.Application.Models;
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
        private readonly IEventBus _bus;


        public RegisterCommandHandler(UserManager<IdentityUser> userManager, IEventBus bus)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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
