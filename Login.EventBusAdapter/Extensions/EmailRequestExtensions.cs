using System.Threading.Tasks;
using Login.Domain;

namespace Login.EventBusAdapter.Extensions
{
    public static class EmailRequestExtensions
    {
        public static Task Send(this EmailRequest email)
        {
            //TODO: Send Email
            return Task.CompletedTask;
        }
    }
}
