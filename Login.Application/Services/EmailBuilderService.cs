using System.Text;
using Login.Application.Models;
using Login.Application.Services.Interfaces;
using Login.Domain;
using Microsoft.AspNetCore.Identity;

namespace Login.Application.Services
{
    public class EmailBuilderService : IEmailBuilderService
    {
        private const string TokenBoxStyle = "padding: 6px 16px; border-color: rgb(35, 78, 215); font-size: 20px";

        public EmailRequest BuildAccontConfirmationEmail(ApplicationUser user, string token)
        {
            var subject = "Confirmação de Conta";
            var message = "Utilize o token abaixo para confirmar sua conta:";

            return BuildEmailWithToken(user, subject, message, token);
        }


        public EmailRequest BuildPasswordRecoveryEmail(ApplicationUser user, string token)
        {
            var subject = "Recuperação de Senha";
            var message = "Utilize o token abaixo para redefinir sua senha:";

            return BuildEmailWithToken(user, subject, message, token);
        }


        private EmailRequest BuildEmailWithToken(ApplicationUser user, string subject, string message, string token)
        {
            var emailTemplate = new StringBuilder();
            emailTemplate.AppendLine($"<p>Querido(a) {user.UserName},</p>");
            emailTemplate.AppendLine($"<p>{message}</p>");
            emailTemplate.AppendLine($"<p style=\"{TokenBoxStyle}\">{token}</p>");
            emailTemplate.AppendLine("<p>- MGR</p>");

            return new EmailRequest()
            {
                Subject = subject,
                DestinationEmail = user.Email,
                Template = emailTemplate.ToString()
            };
        }
    }
}
