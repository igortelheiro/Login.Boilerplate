using MGR.Login.Application.Services.Interfaces;
using MGR.Login.Infra.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MGR.Login.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MailController : Controller
    {
        #region Initialize
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailBuilderService _emailBuilder;

        public MailController(UserManager<ApplicationUser> userManager,
                              IEmailBuilderService emailBuilder)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailBuilder = emailBuilder ?? throw new ArgumentNullException(nameof(emailBuilder));
        }
        #endregion


        [HttpPost("AccountConfirmation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendAccountConfirmationEmail([FromQuery] string email)
        {
            try
            {
                var user = await GetUserByEmailAsync(email);

                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var token = EncryptToken(emailConfirmationToken);

                var emailRequest = _emailBuilder.BuildAccontConfirmationEmail(user, token);
                //TODO: Enviar email para um EmailService

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ToProblemDetails(ex));
            }
        }


        [HttpPost("PasswordRecovery")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendPasswordRecoveryEmail([FromQuery] string email)
        {
            try
            {
                var user = await GetUserByEmailAsync(email);

                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var token = EncryptToken(passwordResetToken);

                var emailRequest = _emailBuilder.BuildPasswordRecoveryEmail(user, token);
                //TODO: Enviar email para um EmailService

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ToProblemDetails(ex));
            }
        }


        private async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
            {
                throw new ArgumentException($"Usuário não encontrado através do email {email}");
            }

            return user;
        }

        private static string EncryptToken(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);
            var encodedToken = Base64UrlEncoder.Encode(bytes);
            return encodedToken;
        }

        private static ProblemDetails ToProblemDetails(Exception ex)
        {
            return new ProblemDetails { Title = "Erro ao enviar email", Detail = ex.Message };
        }
    }
}
