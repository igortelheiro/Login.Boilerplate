using MGR.Login.Application.Models;
using MGR.Login.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace MGR.Login.Application.Services
{
    public class EmailBuilderService : IEmailBuilderService
    {
        private const string TokenBoxStyle = "padding: 6px 16px; border-color: rgb(35, 78, 215); font-size: 20px";

        public EmailRequest BuildAccontConfirmationEmail(IdentityUser user, string token)
        {
            var subject = "Confirmação de Conta";
            var message = "Utilize o token abaixo para confirmar sua conta:";

            return BuildEmailWithToken(user, subject, message, token);
        }


        public EmailRequest BuildPasswordRecoveryEmail(IdentityUser user, string token)
        {
            var subject = "Recuperação de Senha";
            var message = "Utilize o token abaixo para redefinir sua senha:";

            return BuildEmailWithToken(user, subject, message, token);
        }


        private EmailRequest BuildEmailWithToken(IdentityUser user, string subject, string message, string token)
        {
            var emailTemplate = new StringBuilder();
            emailTemplate.AppendLine($"<p>Querido(a) {user.UserName},</p>");
            emailTemplate.AppendLine($"<p>{message}</p>");
            emailTemplate.AppendLine($"<p style=\"{TokenBoxStyle}\">{token}</p>");
            emailTemplate.AppendLine("<p>- MGR</p>");

            return new()
            {
                Subject = subject,
                DestinationEmail = user.Email,
                Template = emailTemplate.ToString()
            };
        }
    }
}
