using Microsoft.IdentityModel.Tokens;

namespace StudyProj.WebApp.Auth
{
    public interface IAuthOptions
    {
        /// <summary>
        /// Token issuer
        /// </summary>
        string Issuer { get; }

        /// <summary>
        /// Audience
        /// </summary>
        string Audience { get; }

        /// <summary>
        /// Encryption key
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Token life time minutes
        /// </summary>
        int LifeTime { get; }

        SymmetricSecurityKey GetSymmetricSecurityKey();
    }
}