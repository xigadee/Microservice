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
        /// The configuration.
        /// </summary>
        public IConfiguration Configuration { get; set; }
    }
}
