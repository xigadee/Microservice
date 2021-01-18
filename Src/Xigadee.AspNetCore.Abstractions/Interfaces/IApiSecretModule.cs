using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is used to set and retrieve application secrets for an underlying store.
    /// </summary>
    public interface IApiSecretModule:IApiModuleBase
    {
        /// <summary>
        /// Retrieves a secret from the underlying secret store.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>A tuple indicating whether the key was resolved and the value.</returns>
        Task<(bool success, string value)> Get(string key);

        /// <summary>
        /// Sets a secret in the underlying store
        /// </summary>
        /// <param name="key">The secret key.</param>
        /// <param name="value">The secret value</param>
        /// <returns>Returns true if set successfully.</returns>
        Task<bool> Set(string key, string value);
    }
}
