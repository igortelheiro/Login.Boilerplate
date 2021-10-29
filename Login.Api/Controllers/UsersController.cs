using System;
using System.Threading;
using System.Threading.Tasks;
using Login.Api.Extensions;
using Login.Application.Commands;
using Login.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Login.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        #region Initialize
        private readonly IMediator _mediator;
        private readonly CancellationToken _cancellationToken;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;
        }
        #endregion


        [HttpPost]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<RegisterResult>> Register([FromBody] RegisterCommand command)
        {
            try
            {
                var response = await _mediator.Send(command, _cancellationToken).ConfigureAwait(false);
                return Created(response.NewUserId, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToProblemDetails("Erro ao tentar registrar novo usuário"));
            }
        }


        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginCommand command)
        {
            try
            {
                var response = await _mediator.Send(command, _cancellationToken).ConfigureAwait(false);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToProblemDetails("Erro ao tentar fazer login"));
            }
        }


        [HttpPost("RefreshToken")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Post))]
        public async Task<ActionResult<LoginResult>> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            try
            {
                var response = await _mediator.Send(command, _cancellationToken).ConfigureAwait(false);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToProblemDetails("Erro ao gerar novo token"));
            }
        }


        [HttpPost("Confirmation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmAccount([FromBody] ConfirmAccountCommand command)
        {
            try
            {
                await _mediator.Send(command, _cancellationToken).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToProblemDetails("Erro ao confirmar conta"));
            }
        }


        [HttpPost("PasswordRecovery")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            try
            {
                var result = await _mediator.Send(command, _cancellationToken).ConfigureAwait(false);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToProblemDetails("Erro ao redefinir senha"));
            }
        }
    }
}
