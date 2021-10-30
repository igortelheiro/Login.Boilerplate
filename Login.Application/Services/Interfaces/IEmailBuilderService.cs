using Login.Application.Models;
using Login.Domain;
using Microsoft.AspNetCore.Identity;

namespace Login.Application.Services.Interfaces
{
    public interface IEmailBuilderService
    {
        EmailRequest BuildPasswordRecoveryEmail(IdentityUser user, string token);
        EmailRequest BuildAccontConfirmationEmail(IdentityUser user, string token);
    }
}
