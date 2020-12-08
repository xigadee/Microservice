using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class holds the details of a user session on the system.
    /// This ensures that timeouts can be managed on the core system and internal user details are not exposed to the client.
    /// </summary>
    /// <seealso cref="Xigadee.EntityAuditableBase" />
    /// <seealso cref="Xigadee.IPropertyBag" />
    public class UserSession : UserReferenceBase
    {
        private const string cnClaimPrefix = "claim_";

        /// <summary>
        /// Gets or sets the source that created the session.
        /// </summary>
        [EntityPropertyHint("sessionsource")]
        public string Source { get; set; }

        /// <summary>
        /// This boolean property is set to true when a customer claim is added.
        /// If this is added, then the Authentication Handler will check for custom claims and add them
        /// to the Claims Identity.
        /// </summary>
        public bool HasCustomClaims { get; set; }
        /// <summary>
        /// This method adds a custom claim to the user session.
        /// </summary>
        /// <param name="claimType"></param>
        /// <param name="claimValue"></param>
        public void AddCustomClaim(string claimType, string claimValue)
        {
            HasCustomClaims = true;
            this.PropertiesSet($"{cnClaimPrefix}{claimType}", claimValue);
        }

        /// <summary>
        /// This is the set of custom claims.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<(string type, string value)> CustomClaims()
        {
            foreach (var prop in Properties)
                if (prop.Key.StartsWith(cnClaimPrefix))
                    yield return (prop.Key.Substring(cnClaimPrefix.Length), prop.Value);
        }

        /// <summary>
        /// This is the optional expriy date of the session. This is used primarily when 
        /// waiting for a 2FA code to be entered for a pending authenticated session.
        /// </summary>
        public DateTime? DateExpiry { get; set; }

        /// <summary>
        /// This class holds the 2FA information when a user is waiting to confirm their logon.
        /// </summary>
        public Session2FA Session2FA { get; set; }

        public bool Has2FACode => Session2FA != null;
    }

    /// <summary>
    /// This class is used to hold the 2FA verification code.
    /// </summary>
    public class Session2FA
    {
        /// <summary>
        /// The id of the pending user.
        /// </summary>
        public Guid UserIdPending { get; set; } 
        /// <summary>
        /// The hash of the 2FA code to compare against.
        /// </summary>
        public string Hash2FACode { get; set; }

        /// <summary>
        /// This is the number of attempts to verify the 2FA code.
        /// </summary>
        public int VerificationAttempts { get; set; } = 0;
        /// <summary>
        /// This is the optional expriy date of the session. This is used primarily when 
        /// waiting for a 2FA code to be entered for a pending authenticated session.
        /// </summary>
        public DateTime? DateExpiry { get; set; }
    }
}
