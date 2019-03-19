using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Kanbanboard.Model;
using Kanbanboard.ViewModels.Auth;
using Microsoft.Extensions.Options;

namespace Kanbanboard.Auth
{
    public class JwtFactory
    {
        private JwtIssuerOptions _options;

        public JwtFactory(IOptions<JwtIssuerOptions> options)
        {
            _options = options.Value;
        }

        public AccessTokenViewModel CreateToken(AppUser user)
        {
            Claim[] claims = {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            };

            var token = new JwtSecurityToken(
                _options.Issuer,
                _options.Audience,
                claims,
                expires: DateTime.UtcNow.AddSeconds(_options.ValidFor),
                signingCredentials: _options.SigningCredentials
            );

            return new AccessTokenViewModel {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = ((DateTimeOffset)token.ValidTo).ToUnixTimeSeconds()
            };
        }
    }
}