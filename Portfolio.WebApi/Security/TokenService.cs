using Microsoft.IdentityModel.Tokens;
using Portfolio.DAL.Models;
using Portfolio.WebApi.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Portfolio.WebApi.Security
{
    public class TokenService : ITokenService
    {
        private readonly IAuthOptions _authOptions;
        public TokenService(IAuthOptions authOptions)
        {
            _authOptions = authOptions;
        }

        public string GenerateToken(Employee employee)
        {
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: _authOptions.Issuer,
                    audience: _authOptions.Audience,
                    notBefore: now,
                    claims: GetIdentity(employee)?.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(_authOptions.AccessTokenLifeTimeMinutes)),
                    signingCredentials: new SigningCredentials(_authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
        public string GenerateRefreshToken(Employee employee)
        {
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: _authOptions.Issuer,
                    audience: _authOptions.Audience,
                    notBefore: now,
                    claims: GetIdentity(employee.Name)?.Claims,
                    expires: now.Add(TimeSpan.FromHours(_authOptions.RefreshTokenLifeTimeHours)),
                    signingCredentials: new SigningCredentials(_authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public JwtSecurityToken GetJwtSecurityToken(string token, bool validateLifetime)
        {
            var tokenValidationParameters = _authOptions.GetTokenValidationParameters(validateLifetime);
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return tokenHandler.ReadJwtToken(token);
        }

        private ClaimsIdentity GetIdentity(Employee employee)
        {
            if (employee != null)
            {
                var claims = new List<Claim>
                {
                    new Claim("name", employee.Name),
                    new Claim("role", employee.Role.ToString().ToLower())
                };

                return new ClaimsIdentity(claims, "Token", "name", "role");
            }

            return null;
        }

        private ClaimsIdentity GetIdentity(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var claims = new List<Claim>
                {
                    new Claim("name", name),
                    new Claim("type", "refresh")
                };

                return new ClaimsIdentity(claims, "Token", "name", "role");
            }

            return null;
        }
    }
}
