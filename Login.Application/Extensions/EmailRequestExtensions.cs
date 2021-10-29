using System.Threading.Tasks;
using MGR.EventBus.Events;
using MGR.EventBus.Interfaces;

namespace Login.Application.Extensions
{
    public static class EmailRequestExtensions
    {
        public static async Task Send(this EmailRequestedEvent email, IEventBus bus) =>
            await bus.Publish(email);
    }
}
