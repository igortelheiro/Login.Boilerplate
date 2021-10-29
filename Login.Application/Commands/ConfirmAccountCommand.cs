using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Login.Application.Commands
{
    public record ConfirmAccountCommand : IRequest
    {
        [EmailAddress]
        [Required] public string Email { get; set; }

        [Required] public string Token { get; set; }
    }
}
