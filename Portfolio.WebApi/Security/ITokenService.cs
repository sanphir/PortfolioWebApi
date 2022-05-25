using Portfolio.DAL.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Portfolio.WebApi.Security
{
    public interface ITokenService
    {
        string GenerateToken(Employee employee);
        string GenerateRefreshToken(Employee employee);
        JwtSecurityToken GetJwtSecurityToken(string token, bool validateLifetime);
    }
}
