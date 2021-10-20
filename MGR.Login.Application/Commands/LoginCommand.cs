using MediatR;
using MGR.Login.Application.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MGR.Login.Application.Commands
{
    public record LoginCommand : IRequest<LoginResult>
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [PasswordPropertyText]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
