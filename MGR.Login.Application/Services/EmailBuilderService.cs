using System.Text;
using MGR.Login.Application.Models;
using MGR.Login.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace MGR.Login.Application.Services
{
    public class EmailBuilderService : IEmailBuilderService
    {
        private const string DefaultUrl = "https://www.localhost:5003";
        private const string AccountConfirmationEndpoint = "Confirmation";
        private const string PasswordRecoveryEndpoint = "Password";
        private const string ButtonStyle = "padding: 6px 16px; background-color: rgb(35, 78, 215); color: #FFFFFF; font-style: none;";
        

        public EmailRequestModel BuildAccontConfirmationEmail(IdentityUser user, string token)
        {
            var subject = "Confirmação de Conta";
            var message = "Favor clicar no botão abaixo para confirmar sua conta:";
            var buttonText = "Confirmar Conta";
            var buttonLink = $"{DefaultUrl}/{AccountConfirmationEndpoint}?email={user.Email}&token={token}";

            return BuildEmail(user, subject, message, buttonText, buttonLink);
        }


        public EmailRequestModel BuildPasswordRecoveryEmail(IdentityUser user, string token)
        {
            var subject = "Recuperação de Senha";
            var message = "Favor clicar no botão abaixo para redefinir sua senha:";
            var buttonText = "Redefinir Senha";
            var buttonLink = $"{DefaultUrl}/{PasswordRecoveryEndpoint}?email={user.Email}&token={token}";

            return BuildEmail(user, subject, message, buttonText, buttonLink);
        }


        private EmailRequestModel BuildEmail(IdentityUser user, string subject, string message, string buttonText, string link)
        {
            var emailTemplate = new StringBuilder();
            emailTemplate.AppendLine($"<p>Querido(a) {user.UserName},</p>");
            emailTemplate.AppendLine($"<p>{message}</p>");
            emailTemplate.AppendLine($"<p><a style=\"{ButtonStyle}\" href=\"{link}\">{buttonText}</a></p>");
            //emailTemplate.AppendLine($"<p>Caso o botão não esteja disponível, acesse o link: {link}</p>");
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
