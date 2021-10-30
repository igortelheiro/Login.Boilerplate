using Login.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using EventBus.Core.Interfaces;
using Login.EventBusAdapter.Extensions;

namespace Login.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MailController : Controller
    {
        #region Initialize
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailBuilderService _emailBuilder;
        private readonly IEventBus _bus;

        public MailController(UserManager<IdentityUser> userManager,
                              IEmailBuilderService emailBuilder,
                              IEventBus bus)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _emailBuilder = emailBuilder ?? throw new ArgumentNullException(nameof(emailBuilder));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }
        #endregion


        [HttpPost("AccountConfirmation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendAccountConfirmationEmail([FromQuery] string destinationEmail)
        {
            try
            {
                var user = await GetUserByEmailAsync(destinationEmail);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var email = _emailBuilder.BuildAccontConfirmationEmail(user, token);
                await _bus.Send(email);

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
        public async Task<IActionResult> SendPasswordRecoveryEmail([FromQuery] string destinationEmail)
        {
            try
            {
                var user = await GetUserByEmailAsync(destinationEmail);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var email = _emailBuilder.BuildPasswordRecoveryEmail(user, token);
                await _bus.Send(email);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ToProblemDetails(ex));
            }
        }


        private async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
            if (user == null)
            {
                throw new ArgumentException($"Usuário não encontrado através do email {email}");
            }

            return user;
        }


        private static ProblemDetails ToProblemDetails(Exception ex) =>
            new()
            {
                Title = "Erro ao enviar email",
                Detail = ex.Message
            };
    }
}
