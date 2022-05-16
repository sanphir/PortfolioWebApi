﻿using Microsoft.IdentityModel.Tokens;

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
        int AccessTokenLifeTimeMinutes { get; }

        /// <summary>
        /// Refresh token life time in hours
        /// </summary>
        int RefreshTokenLifeTimeHours { get; }

        TokenValidationParameters GetTokenValidationParameters();
        SymmetricSecurityKey GetSymmetricSecurityKey();
    }
}