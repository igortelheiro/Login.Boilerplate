using Login.Application.Models;
using Login.Domain;
using Microsoft.AspNetCore.Identity;

namespace Login.Application.Services.Interfaces
{
    public interface IEmailBuilderService
    {
        EmailRequest BuildPasswordRecoveryEmail(ApplicationUser user, string token);
        EmailRequest BuildAccontConfirmationEmail(ApplicationUser user, string token);
    }
}
