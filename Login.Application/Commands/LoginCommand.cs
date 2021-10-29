using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Login.Application.Models;
using MediatR;

namespace Login.Application.Commands
{
    public record LoginCommand : IRequest<LoginResult>
    {
        [EmailAddress]
        [Required] public string Email { get; set; }

        [PasswordPropertyText]
        [Required] public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
