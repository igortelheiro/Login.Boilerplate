using MGR.Login.Application.Models;
using System.Threading.Tasks;

namespace MGR.Login.Application.Extensions
{
    public static class EmailRequestExtensions
    {
        //TODO: Enviar email através de um EmailService
        public static Task Send(this EmailRequest email) =>
            Task.CompletedTask;
    }
}
