using Login.Domain;
using System.Threading.Tasks;

namespace Login.Application.Extensions
{
    public static class EmailRequestExtensions
    {
        public static Task Send(this EmailRequest email)
        {
            //TODO: Enviar email
            return Task.CompletedTask;
        }
    }
}
