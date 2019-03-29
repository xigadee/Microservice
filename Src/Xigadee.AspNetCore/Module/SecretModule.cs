using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This is the default secrets module. It does not store or retrieve sensitive information.
    /// </summary>
    /// <seealso cref="Xigadee.IApiSecretModule" />
    public class SecretModule : ApiModuleBase, IApiSecretModule
    {
        /// <summary>
        /// Retrieves a secret from the underlying secret store.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// A tuple indicating whether the key was resolved and the value.
        /// </returns>
        public Task<(bool success, string value)> Get(string key)
        {
            return Task.FromResult((false, (string)null));
        }
        /// <summary>
        /// Sets a secret in the underlying store
        /// </summary>
        /// <param name="key">The secret key.</param>
        /// <param name="value">The secret value</param>
        /// <returns>
        /// Returns true if set successfully. This does nothing in this module.
        /// </returns>
        public Task<bool> Set(string key, string value)
        {
            return Task.FromResult(false);
        }
    }
}
