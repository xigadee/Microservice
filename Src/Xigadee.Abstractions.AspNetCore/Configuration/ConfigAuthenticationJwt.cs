using System;

namespace Xigadee
{
    /// <summary>
    /// This class contains the JWT token configuration settings.
    /// </summary>
    public class ConfigAuthenticationJwt: ConfigAuthentication
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigAuthenticationJwt"/> class.
        /// </summary>
        public ConfigAuthenticationJwt() { }

        /// <summary>
        /// Gets or sets the name for the authentication method.
        /// </summary>
        public override string Name { get; set; }

        /// <summary>
        /// Gets or sets the token signing key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Number of days until the token expires where warnings will generated
        /// </summary>
        public int LifetimeWarningDays { get; set; } = 7;

        /// <summary>
        /// Number of days until the token expires where critical errors will generated
        /// </summary>
        public int LifetimeCriticalDays { get; set; } = 1;

        /// <summary>
        /// Gets or sets the token audience.
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// Gets or sets the token issuer.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the validate token expiry option.
        /// </summary>
        public bool? ValidateTokenExpiry { get; set; }

        /// <summary>
        /// Gets or sets the permitted token skew time in seconds.
        /// </summary>
        public int? TokenExpirySkewInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the default token validity time period when creating a token.
        /// </summary>
        public TimeSpan? DefaultTokenValidity { get; set; }

        /// <summary>
        /// Gets the clock skew, which is the permitted time difference between system time and token time.
        /// The default is 10 seconds.
        /// </summary>
        public TimeSpan GetClockSkew()
        {
            return
                TokenExpirySkewInSeconds.HasValue
                    ? TimeSpan.FromSeconds(TokenExpirySkewInSeconds.Value)
                    : TimeSpan.FromSeconds(10);
        }
    }
}
