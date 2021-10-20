using System.ComponentModel.DataAnnotations;
using MediatR;
using MGR.Login.Application.Models;

namespace MGR.Login.Application.Commands
{
    public record RefreshTokenCommand : IRequest<LoginResult>
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
