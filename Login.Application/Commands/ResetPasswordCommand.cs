using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Login.Application.Commands
{
    public record ResetPasswordCommand : IRequest
    {
        [EmailAddress]
        [Required] public string Email { get; set; }
        [Required] public string Token { get; set; }
        [Required] public string NewPassword { get; set; }
    }
}
