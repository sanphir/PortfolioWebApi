using StudyProj.DAL.Models;
using System.IdentityModel.Tokens.Jwt;

namespace StudyProj.WebApp.Security
{
    public interface ITokenService
    {
        string GenerateToken(Employee employee);
        string GenerateRefreshToken(Employee employee);
        JwtSecurityToken GetJwtSecurityToken(string token, bool validateLifetime);
    }
}
