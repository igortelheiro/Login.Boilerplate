using System;
using Microsoft.IdentityModel.Tokens;

namespace Login.Application.Models
{
    public class JwtConfiguration
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
        public DateTime Expiry { get; set; }
    }
}
