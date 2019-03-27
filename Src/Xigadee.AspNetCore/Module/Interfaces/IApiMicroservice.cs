using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This is the root API-based microservice application interface.
    /// </summary>
    public interface IApiMicroservice
    {
        /// <summary>
        /// Initializes the module with the application environment settings.
        /// </summary>
        /// <param name="env">The environment.</param>
        void Initialize(IHostingEnvironment env = null);

        /// <summary>
        /// Gets or sets the application configuration.
        /// </summary>
        IConfiguration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the hosting environment.
        /// </summary>
        IHostingEnvironment Environment { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// Connects the application components and registers the relevant setvices.
        /// </summary>
        /// <param name="lf">The logger factory.</param>
        void Connect(ILoggerFactory lf);

    }
    /// <summary>
    /// This interface is implemented by applications that use Xigadee in an API or web based application.
    /// </summary>
    /// <typeparam name="MODSEC">The type of the security module.</typeparam>
    /// <typeparam name="CONATEN">The authentication module type.</typeparam>
    /// <typeparam name="CONATHZ">The authorization module type.</typeparam>
    public interface IApiMicroservice<MODSEC, CONATEN, CONATHZ>: IApiMicroservice
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
