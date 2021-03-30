using Microsoft.Extensions.Configuration;

namespace Xigadee
{
    /// <summary>
    /// This is the host container base. This class abstracts the common properties from the 
    /// underlying hosting component.
    /// </summary>
    public abstract class HostingContainerBase
    {
        /// <summary>
        /// The host content root path.
        /// </summary>
        public abstract string ContentRootPath { get; }
        /// <summary>
        /// The envionment name used for config resolution.
        /// </summary>
        public abstract string EnvironmentName { get; }
        /// <summary>
        /// This is the application name.
        /// </summary>
        public abstract string ApplicationName { get; }
        /// <summary>
        /// This is the machine name.
        /// </summary>
        public virtual string MachineName { get; } = System.Environment.MachineName;
        /// <summary>
        /// The configuration.
        /// </summary>
        public IConfiguration Configuration { get; set; }
        /// <summary>
        /// This is the base Url of the host container
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// This is the unique id of the instance.
        /// </summary>
        public virtual string InstanceId {get;set;}
    }
}
