using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Xigadee
{
    /// <summary>
    /// This is the static user token helper.
    /// </summary>
    public static class JwtTokenHelper
    {
        /// <summary>
        /// Creates the symmetric token.
        /// </summary>
        /// <param name="Jwt">The JWT.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="tokenExpiry">The token expiry.</param>
        /// <param name="extendedClaims">The extended claims collection.</param>
        /// <returns>Returns the token as a string.</returns>
        public static string CreateSymmetricHmacSha256(ConfigAuthenticationJwt Jwt
            , Guid id
            , TimeSpan? tokenExpiry = null
            , IEnumerable<Claim> extendedClaims = null)
        {
            return CreateSymmetricHmacSha256(Jwt
                , id
                , Convert.FromBase64String(Jwt.Key)
                , tokenExpiry
                , extendedClaims);
        }

        /// <summary>
        /// Creates the symmetric token.
        /// </summary>
        /// <param name="Jwt">The JWT.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="key">The symmetric key.</param>
        /// <param name="tokenExpiry">The token expiry.</param>
        /// <param name="extendedClaims">The extended claims collection.</param>
        /// <returns>Returns the token as a string.</returns>
        public static string CreateSymmetricHmacSha256(ConfigAuthenticationJwt Jwt
            , Guid id
            , byte[] key
            , TimeSpan? tokenExpiry = null
            , IEnumerable<Claim> extendedClaims = null)
        {
            var skey = new SymmetricSecurityKey(key);
            var creds = new SigningCredentials(skey, SecurityAlgorithms.HmacSha256);

            return WriteToken(id, creds, Jwt, extendedClaims, tokenExpiry ?? Jwt.DefaultTokenValidity ?? TimeSpan.FromDays(1));
        }

        /// <summary>
        /// Creates the symmetric token.
        /// </summary>
        /// <param name="Jwt">The JWT.</param>
        /// <param name="cert">The signing certificate.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="tokenExpiry">The token expiry.</param>
        /// <param name="extendedClaims">The extended claims collection.</param>
        /// <returns>Returns the token as a string.</returns>
        public static string CreateAsymmetricRsaSha512(ConfigAuthenticationJwt Jwt
            , X509Certificate2 cert
            , Guid id
            , TimeSpan? tokenExpiry = null
            , IEnumerable<Claim> extendedClaims = null)
        {
            var skey = new X509SecurityKey(cert);
            var creds = new SigningCredentials(skey, SecurityAlgorithms.RsaSha512);

            return WriteToken(id, creds, Jwt, extendedClaims, tokenExpiry ?? Jwt.DefaultTokenValidity ?? TimeSpan.FromDays(1));
        }

        /// <summary>
        /// Writes the JWT token.
        /// </summary>
        /// <param name="sid">The sid identifier.</param>
        /// <param name="creds">The signing credentials.</param>
        /// <param name="Jwt">The JWT configuration.</param>
        /// <param name="extendedClaims">The optional extended claims.</param>
        /// <param name="tokenExpiry">The token expiry.</param>
        /// <returns>Returns a JWT formatted token</returns>
        public static string WriteToken(Guid sid, SigningCredentials creds, ConfigAuthenticationJwt Jwt, IEnumerable<Claim> extendedClaims, TimeSpan tokenExpiry)
        {
            var claims = new Claim[] { new Claim(ClaimTypes.Sid, sid.ToString("N")) };
            if (extendedClaims == null)
                claims = claims.Union(extendedClaims).ToArray();

            var token = new JwtSecurityToken(
                issuer: Jwt.Issuer,
                audience: Jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(tokenExpiry),
                signingCredentials: creds);

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(token);
        }
    }
}
