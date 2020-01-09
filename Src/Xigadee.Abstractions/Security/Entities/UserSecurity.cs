using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class is used to store the authentication details for a user on the system.
    /// </summary>
    /// <seealso cref="Xigadee.EntityAuditableBase" />
    /// <seealso cref="Xigadee.IPropertyBag" />
    public class UserSecurity : EntityAuditableBase, IPropertyBag
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSecurity"/> class.
        /// </summary>
        public UserSecurity()
        {

        } 
        /// <summary>
        /// Initializes a new instance of the <see cref="UserSecurity"/> class.
        /// </summary>
        /// <param name="ipAddresses">The permitted IP addresses.</param>
        /// <param name="certificates">The permitted certificates.</param>
        public UserSecurity( IEnumerable<KeyValuePair<Guid, string>> ipAddresses, IEnumerable<KeyValuePair<Guid, string>> certificates)
        {
            foreach (var item in ipAddresses)
                IPAddresses.Add(item.Key, item.Value);

            foreach (var item in certificates)
                Certificates.Add(item.Key, item.Value);
        }
        #endregion

        /// <summary>
        /// Gets or sets the authentication collection.
        /// </summary>
        public Dictionary<string, string> Authentication { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// The property bag container dictionary. This has a set of extensible properties for the user.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Indicates whether the security record is active or not
        /// </summary>
        [EntityPropertyHint("isactive")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// The state property can be used to indicate to systems when a user
        /// requires a certain condition to be completed, i.e. post registration or clean up.
        /// </summary>
        [EntityPropertyHint("state")]
        public string State { get; set; }

        /// <summary>
        /// Records any IP address restrictions.
        /// </summary>
        public Dictionary<Guid, string> IPAddresses { get; } = new Dictionary<Guid, string>();
        /// <summary>
        /// Gets any certificate thumbprint matches.
        /// </summary>
        public Dictionary<Guid, string> Certificates { get; } = new Dictionary<Guid, string>();
    }
}
