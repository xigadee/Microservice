using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This module can be used to retrieve certificates. 
    /// </summary>
    public interface IApiCertificateModule:IApiModuleBase
    {
        /// <summary>
        /// Resolves a certificate from a specified thumbprint.
        /// </summary>
        /// <param name="thumbprint">The cert thumbprint.</param>
        /// <returns>Returns a tuple with the success indicator and certificate.</returns>
        Task<(bool success, X509Certificate2 cert)> TryResolve(string thumbprint);
    }
}
