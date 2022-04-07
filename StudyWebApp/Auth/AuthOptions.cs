using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace StudyProj.WebApp.Auth
{
    public class AuthOptions : IAuthOptions
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;
        private readonly int _lifeTime;

        /// <summary>
        /// Token issuer
        /// </summary>
        public string Issuer => _issuer;

        /// <summary>
        /// Audience
        /// </summary>
        public string Audience => _audience;

        /// <summary>
        /// Encryption key
        /// </summary>
        public string Key => _key;

        /// <summary>
        /// Token life time minutes
        /// </summary>
        public int LifeTime => _lifeTime;

        public AuthOptions(IConfiguration configuration)
        {
            _issuer = configuration.GetSection("AuthOptions:Issuer")?.Value ?? throw new ArgumentException("Issuer not specify in the configuration file");
            _audience = configuration.GetSection("AuthOptions:Audience")?.Value ?? throw new ArgumentException("Audience not specify in the configuration file");
            _key = configuration.GetSection("AuthOptions:Key")?.Value ?? throw new ArgumentException("Token key not specify in the configuration file");
            if (!int.TryParse(configuration.GetSection("AuthOptions:LifeTime")?.Value ?? "", out _lifeTime))
            {
                throw new ArgumentException("LifeTime not specify in the configuration file");
            }
        }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}
