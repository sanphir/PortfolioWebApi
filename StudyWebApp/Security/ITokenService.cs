using System.Security.Claims;

namespace StudyProj.WebApp.Security
{
    public interface ITokenService
    {
        string GenerateToken(ClaimsIdentity identity);
    }
}
