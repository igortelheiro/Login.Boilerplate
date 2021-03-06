using System.ComponentModel.DataAnnotations;
using Login.Application.Models;
using MediatR;

namespace Login.Application.Commands
{
    public record RefreshTokenCommand : IRequest<LoginResult>
    {
        [EmailAddress]
        [Required] public string Email { get; set; }
        [Required] public string RefreshToken { get; set; }
    }
}
