using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Login.Application.Models;
using MediatR;

namespace Login.Application.Commands
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
