using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Login.Application.Models;
using MediatR;

namespace Login.Application.Commands
{
    public class RegisterCommand : IRequest<RegisterResult>
    {
        [Required]
        [StringLength(50, ErrorMessage = "O Nome deve ter entre {2} e {1} caracteres", MinimumLength = 3)]
        public string NomeCompleto { get; set; }

        [Required]
        public int CondominioId { get; set; }

        [Required]
        [StringLength(5, ErrorMessage = "O Nome deve ter entre {2} e {1} caracteres", MinimumLength = 1)]
        public string Bloco { get; set; }

        [Required]
        public int NumeroApto { get; set; }

        [Required]
        public int PerfilAcesso { get; set; }

        public string UserName { get; set; }


        [StringLength(30, ErrorMessage = "O número deve ter entre {2} e {1} caracteres", MinimumLength = 8)]
        [Phone] public string PhoneNumber { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "O email deve ter entre {2} e {1} caracteres", MinimumLength = 6)]
        [EmailAddress] public string Email { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "A senha deve ter entre {2} e {1} caracteres", MinimumLength = 6)]
        [PasswordPropertyText] public string Password { get; set; }


        [Required]
        [Compare(nameof(Password), ErrorMessage = "A senha e a confirmação de senha não combinam")]
        [PasswordPropertyText] public string ConfirmPassword { get; set; }
    }
}
