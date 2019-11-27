using System;

namespace Xigadee
{
    /// <summary>
    /// This is the identity of the application
    /// </summary>
    public class ApiServiceIdentity: IApiServiceIdentity
    {
        /// <summary>
        /// The application identity parts.
        /// </summary>
        /// <param name="id">The unique id of the instance.</param>
        /// <param name="machineName">The current machine name.</param>
        /// <param name="application">The application name.</param>
        /// <param name="version">The current application version.</param>
        /// <param name="domain">The current domain of the hosting platform.</param>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="environmentName">The environment name.</param>
        public ApiServiceIdentity(Guid id,
            string machineName, string application, string version, string domain
            , string instanceId, string environmentName)
        {
            Id = id;
            MachineName = machineName;
            Application = application;
            Version = version;
            Domain = domain;
            InstanceId = instanceId;
            EnvironmentName = environmentName;
        }

        /// <summary>
        /// Gets the unique identifier for the instance.
        /// </summary>
        public Guid Id { get; } 
        /// <summary>
        /// Gets or sets the name of the machine.
        /// </summary>
        public string MachineName { get; }
        /// <summary>
        /// Gets or sets the name of the machine.
        /// </summary>
        public string InstanceId { get; }

        /// <summary>
        /// The environment name taken from ASPNETCORE_ENVIRONMENTNAME environment variable
        /// </summary>
        public string EnvironmentName { get; }

        /// <summary>
        /// The JWT Audience that this microservice authenticates against
        /// </summary>
        public string JwtAudience { get; }

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        public string Application { get; }
        /// <summary>
        /// Gets or sets the assembly version.
        /// </summary>
        public string Version { get; }
        /// <summary>
        /// Gets the start time for the application.
        /// </summary>
        public DateTime Start { get; } = DateTime.UtcNow;
        /// <summary>
        /// Gets the combined string.
        /// </summary>
        public string Combined => $"{Application}_{Version}_{Id.ToString("N").ToUpperInvariant()}_{Start:yyyy-MM-ddTHH:mm:ssZ}";
        /// <summary>
        /// Gets the domain if this is hosted in Azure.
        /// </summary>
        public string Domain { get; }
    }
}
