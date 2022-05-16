using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace StudyProj.WebApp.Auth
{
    public class AuthOptions : IAuthOptions
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;
        private readonly int _accessTokenLifeTimeMinutes;
        private readonly int _refreshTokenLifeTimeHours;

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
        /// Access token life time in minutes
        /// </summary>
        public int AccessTokenLifeTimeMinutes => _accessTokenLifeTimeMinutes;

        /// <summary>
        /// Refresh token life time in hours
        /// </summary>
        public int RefreshTokenLifeTimeHours => _refreshTokenLifeTimeHours;

        /// <summary>
        /// Configuration file auth sedction name
        /// </summary>
        private const string AUTH_SECTION_NAME = "AuthOptions";

        public AuthOptions(IConfiguration configuration)
        {
            _issuer = configuration.GetSection($"{AUTH_SECTION_NAME}:Issuer")?.Value ?? throw new ArgumentException("Issuer not specify in the configuration file");
            _audience = configuration.GetSection($"{AUTH_SECTION_NAME}:Audience")?.Value ?? throw new ArgumentException("Audience not specify in the configuration file");
            _key = configuration.GetSection($"{AUTH_SECTION_NAME}:Key")?.Value ?? throw new ArgumentException("Token key not specify in the configuration file");

            if (!int.TryParse(configuration.GetSection($"{AUTH_SECTION_NAME}:AccessTokenLifeTimeMinutes")?.Value ?? "", out _accessTokenLifeTimeMinutes))
            {
                throw new ArgumentException("AccessTokenLifeTimeMinutes not specify in the configuration file");
            }

            if (!int.TryParse(configuration.GetSection($"{AUTH_SECTION_NAME}:RefreshTokenLifeTimeHours")?.Value ?? "", out _refreshTokenLifeTimeHours))
            {
                throw new ArgumentException("RefreshTokenLifeTimeHours not specify in the configuration file");
            }
        }

        public TokenValidationParameters GetTokenValidationParameters(bool validateLifetime = true)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = validateLifetime,

                IssuerSigningKey = GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true,
            };
        }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }
    }
}
