using System.Threading.Tasks;
using EventBus.Core.Events;
using EventBus.Core.Interfaces;
using Login.Domain;

namespace Login.EventBusAdapter.Extensions
{
    public static class EmailRequestExtensions
    {
        public static async Task Send(this IEventBus bus, EmailRequest email)
        {
            var emailRequested = new EmailRequestedEvent
            {
                DestinationEmail = email.DestinationEmail,
                Subject = email.Subject,
                Content = email.Content,
                Template = email.Template
            };

            await bus.Publish(emailRequested);
        }
    }
}
