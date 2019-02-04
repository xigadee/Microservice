using System;

namespace Xigadee
{
    /// <summary>
    /// This class contains the default claims for a JWT token.
    /// </summary>
    /// <seealso cref="Xigadee.ClaimsSet" />
    public class JwtClaims: ClaimsSet
    {
        #region Registered claims        
        /// <summary>
        /// The issuer
        /// </summary>
        public const string HeaderIssuer = "iss";
        /// <summary>
        /// The subject
        /// </summary>
        public const string HeaderSubject = "sub";
        /// <summary>
        /// The audience
        /// </summary>
        public const string HeaderAudience = "aud";
        /// <summary>
        /// The JWT id
        /// </summary>
        public const string HeaderJWTID = "jti";

        /// <summary>
        /// The expiration time
        /// </summary>
        public const string HeaderExpirationTime = "exp";
        /// <summary>
        /// The "not before" date/time
        /// </summary>
        public const string HeaderNotBefore = "nbf";
        /// <summary>
        /// The "issued at" date/time
        /// </summary>
        public const string HeaderIssuedAt = "iat";
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtClaims"/> class.
        /// </summary>
        public JwtClaims():base()
        {

        }
        /// <summary>
        /// This override provides specific claims for the JWTClaims.
        /// </summary>
        /// <param name="json"></param>
        public JwtClaims(string json) : base(json)
        {
        }


        /// <summary>
        /// Gets or sets the JWT identifier.
        /// </summary>
        public string JWTId { get { return GetClaim<string>(HeaderJWTID);} set { base[HeaderJWTID] = value; } }
        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        public string Issuer { get { return GetClaim<string>(HeaderIssuer); } set { base[HeaderIssuer] = value; } }
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        public string Subject { get { return GetClaim<string>(HeaderSubject); } set { base[HeaderSubject] = value; } }
        /// <summary>
        /// Gets or sets the audience.
        /// </summary>
        public string Audience { get { return GetClaim<string>(HeaderAudience); } set { base[HeaderAudience] = value; } }
        /// <summary>
        /// Gets or sets the expiration time.
        /// </summary>
        public DateTime? ExpirationTime { get { return JwtHelper.FromNumericDate(GetClaim<long?>(HeaderExpirationTime)); } set { base[HeaderExpirationTime] = JwtHelper.ToNumericDate(value); } }
        /// <summary>
        /// Gets or sets the not before date/time.
        /// </summary>
        public DateTime? NotBefore { get { return JwtHelper.FromNumericDate(GetClaim<long?>(HeaderNotBefore)); } set { base[HeaderNotBefore] = JwtHelper.ToNumericDate(value); } }
        /// <summary>
        /// Gets or sets the issued at date/time.
        /// </summary>
        public DateTime? IssuedAt { get { return JwtHelper.FromNumericDate(GetClaim<long?>(HeaderIssuedAt)); } set { base[HeaderIssuedAt] = JwtHelper.ToNumericDate(value); } }

    }
}
