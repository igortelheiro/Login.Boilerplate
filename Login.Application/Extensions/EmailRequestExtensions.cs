using EventBus.Core.Events;
using IntegrationEventLogEF;
using IntegrationEventLogEF.Services;
using Login.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Login.Application.Extensions
{
    public static class EmailRequestExtensions
    {
        public static async Task Send(this EmailRequest email, IServiceProvider sp)
        {
            var eventLogService = sp.GetRequiredService<IIntegrationEventLogService>();
            var dbContext = sp.GetRequiredService<IntegrationEventLogContext>();

            var transactionId = dbContext.Database.CurrentTransaction?.TransactionId ?? Guid.NewGuid();
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;

            var emailRequested = new EmailRequestedEvent
            {
                DestinationEmail = email.DestinationEmail,
                Subject = email.Subject,
                Content = email.Content,
                Template = email.Template
            };
            await eventLogService.SaveEventLogAsync(emailRequested, transactionId, cancellationToken);
        }
    }
}
