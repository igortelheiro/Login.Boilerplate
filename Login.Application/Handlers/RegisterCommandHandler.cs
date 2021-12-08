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
using Login.EntityFrameworkAdapter.Context;
using IntegrationEventLogEF.Services;
using IntegrationEventLogEF.Utilities;
using EventBus.Core.Events;

namespace Login.Application.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
    {
        #region Initialize
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailBuilderService _emailBuilder;
        private readonly IServiceProvider _serviceProvider;
        private readonly IIntegrationEventLogService _eventLogService;
        private readonly ApplicationDbContext _dbContext;

        public RegisterCommandHandler(UserManager<IdentityUser> userManager,
                                      IEmailBuilderService emailBuilder,
                                      IServiceProvider serviceProvider,
                                      IIntegrationEventLogService eventLogService,
                                      ApplicationDbContext dbContext)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailBuilder = emailBuilder ?? throw new ArgumentNullException(nameof(emailBuilder));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        #endregion


        public async Task<RegisterResult> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            IdentityUser newUser = null;

            await ResilientTransaction.New(_dbContext).ExecuteAsync(async _ =>
            {
                newUser = await CreateUserAsync(command);
                var userCreated = new UserCreatedEvent
                {
                    UserId = newUser.Id,
                    Name = newUser.UserName,
                    Email = newUser.Email
                };
                await _eventLogService.SaveEventLogAsync(userCreated, _dbContext.Database.CurrentTransaction.TransactionId, cancellationToken);

                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var email = _emailBuilder.BuildAccontConfirmationEmail(newUser, confirmationToken);
                await email.Send(_serviceProvider);
            });

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
