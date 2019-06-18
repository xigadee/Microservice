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
    }
}
