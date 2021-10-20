using Microsoft.IdentityModel.Tokens;
using System;

namespace MGR.Login.Common
{
    public class JwtConfiguration
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
        public DateTime Expiry { get; set; }
    }
}
