using EventBus.Core.Events;
using EventBus.Core.Interfaces;
using Login.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Login.Application.Extensions
{
    public static class EmailRequestExtensions
    {
        public static async Task Send(this EmailRequest email, IServiceProvider sp)
        {
            var eventBus = sp.GetRequiredService<IEventBus>();

            var emailEvent = new EmailRequestedEvent()
            {
                DestinationEmail = email.DestinationEmail,
                Content = email.Content,
                Subject = email.Subject,
                Template = email.Template
            };
            await eventBus.Publish(emailEvent);
        }
    }
}
