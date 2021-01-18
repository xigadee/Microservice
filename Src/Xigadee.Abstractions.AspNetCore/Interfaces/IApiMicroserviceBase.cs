using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by applications that use Xigadee in an API or web based application.
    /// </summary>
    /// <typeparam name="MODSEC">The type of the security module.</typeparam>
    /// <typeparam name="CONATEN">The authentication module type.</typeparam>
    /// <typeparam name="CONATHZ">The authorization module type.</typeparam>
    public interface IApiMicroserviceBase<MODSEC, CONATEN, CONATHZ>
        where MODSEC : IApiUserSecurityModule
        where CONATEN : ConfigAuthentication
        where CONATHZ : ConfigAuthorization
    {
        /// <summary>
        /// Gets the configuration authentication.
        /// </summary>
        CONATEN ConfigurationAuthentication { get; }
        /// <summary>
        /// Gets the configuration authorization.
        /// </summary>
        CONATHZ ConfigurationAuthorization { get; }

        /// <summary>
        /// Gets the user security collection, which is used to authenticate a user.
        /// </summary>
        MODSEC UserSecurityModule { get; set; }

        /// <summary>
        /// Gets or sets the Microservice identity.
        /// </summary>
        MicroserviceId Identity { get; set; }

        /// <summary>
        /// Gets or sets the certificate module.
        /// </summary>
        IApiCertificateModule CertificateModule { get; set; }
        /// <summary>
        /// Gets or sets the secret module.
        /// </summary>
        IApiSecretModule SecretModule { get; set; }
    }
}
