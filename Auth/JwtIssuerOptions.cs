using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace Kanbanboard.Auth
{    
    public class JwtIssuerOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ValidFor { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
    }
}