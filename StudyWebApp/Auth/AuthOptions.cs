using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace StudyProj.WebApp.Auth
{
    public class AuthOptions : IAuthOptions
    {

        /// <summary>
        /// Token issuer
        /// </summary>
        public static readonly string Issuer = "NS";

        /// <summary>
        /// Audience
        /// </summary>
        public static readonly string Audience = "ALL";

        /// <summary>
        /// Encryption key
        /// </summary>
        public static readonly string Key = "vs3GO4T2uSGFgvLF";

        /// <summary>
        /// Token life time minutes
        /// </summary>
        public static readonly int LifeTime = 10;


        ///// <summary>
        ///// Token issuer
        ///// </summary>
        //public readonly string Issuer;

        ///// <summary>
        ///// Audience
        ///// </summary>
        //public readonly string Audience;

        ///// <summary>
        ///// Encryption key
        ///// </summary>
        //public readonly string Key;

        ///// <summary>
        ///// Token life time minutes
        ///// </summary>
        //public readonly int LifeTime;

        //public AuthOptions(IConfiguration configuration)
        //{
        //    Issuer = configuration.GetSection("AuthOptions:Issuer")?.Value ?? throw new ArgumentException("Issuer not specify in the configuration file");
        //    Audience = configuration.GetSection("AuthOptions:Audience")?.Value ?? throw new ArgumentException("Audience not specify in the configuration file");
        //    Key = configuration.GetSection("AuthOptions:Key")?.Value ?? throw new ArgumentException("Token key not specify in the configuration file");
        //    if (!int.TryParse(configuration.GetSection("AuthOptions:LifeTime")?.Value ?? "", out LifeTime))
        //    {
        //        throw new ArgumentException("LifeTime not specify in the configuration file");
        //    }
        //}

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}
