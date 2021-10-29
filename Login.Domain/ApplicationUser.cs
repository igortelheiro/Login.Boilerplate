using Microsoft.AspNetCore.Identity;

namespace Login.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string NomeCompleto { get; set; }
        public int CondominioId { get; set; }
        public string Bloco { get; set; }
        public int NumeroApto { get; set; }
        public int PerfilAcesso { get; set; }
    }
}
