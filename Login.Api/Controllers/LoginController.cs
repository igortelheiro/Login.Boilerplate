using Login.Api.Extensions;
using Login.Application.Commands;
using Login.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Login.Api.Controllers
{
    public class LoginController : ControllerBase
    {
        #region Initialize
        private readonly IMediator _mediator;
        private readonly CancellationToken _cancellationToken;

        public LoginController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token;
        }
        #endregion


        [HttpPost]
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
    }
}
