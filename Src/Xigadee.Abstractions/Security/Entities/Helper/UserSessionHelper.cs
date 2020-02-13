using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class provides helper functionality for the User Session object.
    /// </summary>
    public static class UserSessionHelper
    {
        #region Set2fa(this UserSession session, Guid userId, string code, int ver = 1)
        /// <summary>
        /// This static method sets the 2fa code on the UserSession.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="userId">The user id to set.</param>
        /// <param name="code">The code to set.</param>
        /// <param name="ver">The optional version. Currently only 1 is supported.</param>
        public static void Set2fa(this UserSession session, Guid userId, string code, int ver = 1)
        {
            var s2fa = new Session2FA();

            s2fa.UserIdPending = userId;
            //s2fa.Attempts++;

            s2fa.Hash2FACode = S2faHashGenerate(session.Id, userId, code, ver);

            session.Session2FA = s2fa;
        }
        #endregion
        #region Validate2fa(this UserSession session, string code)
        /// <summary>
        /// This method hashes the incoming code with the userGuid.
        /// </summary>
        /// <param name="session">The user session.</param>
        /// <param name="code">The code to validate against.</param>
        /// <returns>Returns true is the data marches.</returns>
        public static bool Validate2fa(this UserSession session, string code)
        {
            if (string.IsNullOrEmpty(code) || session?.Session2FA == null)
                return false;

            var raw = S2faHashGenerate(session.Id, session.Session2FA.UserIdPending, code, 1);

            return session.Session2FA.Hash2FACode == raw;
        }
        #endregion

        #region Create(Guid sessionId, Guid userId, string code, int ver = 1)
        /// <summary>
        /// This creates the confirmation code.
        /// </summary>
        /// <param name="sessionId">The session Id.</param>
        /// <param name="userId">The user Id.</param>
        /// <param name="code">The code.</param>
        /// <param name="ver">The code version. The default is 1.</param>
        /// <returns>Returns the Session 2FA.</returns>
        public static Session2FA Create(Guid sessionId, Guid userId, string code, int ver = 1)
        {
            var s2fa = new Session2FA();

            s2fa.UserIdPending = userId;

            s2fa.Hash2FACode = S2faHashGenerate(sessionId, userId, code, ver);

            return s2fa;
        }
        #endregion

        private static string S2faHashGenerate(Guid sessionId, Guid userId, string code, int ver = 1)
        {
            if (ver != 1)
                throw new ArgumentOutOfRangeException("ver", $"Unsupported hash version {ver} - only ver=1 is currently supported.");

            var raw = $"{sessionId:N}:{userId:N}:{code}".ToUpperInvariant();

            byte[] hash;
            using (var incrementalHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA512))
            {
                incrementalHash.AppendData(Encoding.UTF8.GetBytes(raw));
                hash = incrementalHash.GetHashAndReset();
            }

            return $"v{ver}.{Convert.ToBase64String(hash)}";
        }

    }
}
