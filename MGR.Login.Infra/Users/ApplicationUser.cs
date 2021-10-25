using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MGR.Login.Infra.Users
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
