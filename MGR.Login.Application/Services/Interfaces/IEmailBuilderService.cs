using MGR.Login.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace MGR.Login.Application.Services.Interfaces
{
    public interface IEmailBuilderService
    {
        EmailRequest BuildPasswordRecoveryEmail(IdentityUser user, string token);
        EmailRequest BuildAccontConfirmationEmail(IdentityUser user, string token);
    }
}
