using MGR.Login.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace MGR.Login.Application.Services.Interfaces
{
    public interface IEmailBuilderService
    {
        SendEmailRequest BuildPasswordRecoveryEmail(IdentityUser user, string token);
        SendEmailRequest BuildAccontConfirmationEmail(IdentityUser user, string token);
    }
}
