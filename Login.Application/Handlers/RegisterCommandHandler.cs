using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Login.Application.Commands;
using Login.Application.Models;
using Login.Domain;
using MediatR;
using MGR.EventBus.Events;
using MGR.EventBus.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Login.Application.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
    {
        #region Initialize
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEventBus _bus;


        public RegisterCommandHandler(UserManager<ApplicationUser> userManager, IEventBus bus)
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
    }
}
