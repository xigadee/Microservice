using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This class is used to manage the interactions against the application certificate store, or Azure KeyVault
    /// N.B. This currently assumes that the certificate has been saved into KeyVault using its thumbprint as its name
    /// </summary>
    public class CertificateModule : IApiCertificateModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateModule"/> class.
        /// </summary>
        /// <param name="secretModule">The secret module.</param>
        public CertificateModule(IApiSecretModule secretModule)
        {
            SecretModule = secretModule;
        }

        /// <summary>
        /// Gets the secret module.
        /// </summary>
        protected IApiSecretModule SecretModule { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Resolves the specified thumbprint.
        /// </summary>
        /// <param name="thumbprint">The thumbprint.</param>
        /// <returns></returns>
        public virtual Task<(bool success, X509Certificate2 cert)> TryResolve(string thumbprint)
        {
            try
            {
                return Task.FromResult(GetFromLocalStore(thumbprint));
            }
            catch (Exception ex)
            {
                Logger?.LogError($"Unable to retrieve certificate with thumbprint {thumbprint}", ex);
            }

            return Task.FromResult((false, (X509Certificate2)null));
        }

        private (bool success, X509Certificate2 cert) GetFromLocalStore(string thumbprint)
        {
            using (var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);
                var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (certCollection.Count == 0)
                    return (false, null);
                return (true, certCollection[0]);
            }
        }

    }

}
