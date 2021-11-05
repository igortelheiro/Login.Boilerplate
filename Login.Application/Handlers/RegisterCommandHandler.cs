using EventBus.Core.Events;
using EventBus.Core.Interfaces;
using Login.Application.Commands;
using Login.Application.Models;
using Login.Application.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IntegrationEventLogEF.Utilities;
using Login.Application.Extensions;
using Login.Domain;
using Login.EntityFrameworkAdapter.Context;

namespace Login.Application.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
    {
        #region Initialize
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailBuilderService _emailBuilder;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBus _bus;
        private readonly ApplicationDbContext _dbContext;

        public RegisterCommandHandler(UserManager<ApplicationUser> userManager,
                                      IEmailBuilderService emailBuilder,
                                      IServiceProvider serviceProvider,
                                      IEventBus bus,
                                      ApplicationDbContext dbContext)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailBuilder = emailBuilder ?? throw new ArgumentNullException(nameof(emailBuilder));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        #endregion


        public async Task<RegisterResult> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            ApplicationUser newUser = null;

            await ResilientTransaction.New(_dbContext).ExecuteAsync(async _ =>
            {
                newUser = await CreateUserAsync(command);
                var userCreated = new UserCreatedEvent
                {
                    UserId = newUser.Id,
                    Name = newUser.UserName,
                    Email = newUser.Email
                };
                await _bus.PublishAsync(userCreated);

                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var email = _emailBuilder.BuildAccontConfirmationEmail(newUser, confirmationToken);
                await email.Send(_serviceProvider);
            });

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
                NumeroApto = command.NumeroApto
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
