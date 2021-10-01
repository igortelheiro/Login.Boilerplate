using MGR.Login.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace MGR.Login.Application.Services.Interfaces
{
    public interface IEmailBuilderService
    {
        EmailRequestModel BuildPasswordRecoveryEmail(IdentityUser user, string token);
        EmailRequestModel BuildAccontConfirmationEmail(IdentityUser user, string token);
    }
}
