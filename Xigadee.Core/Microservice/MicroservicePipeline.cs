using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This channel is used by extension methods to create a simple channel based configuration.
    /// </summary>
    /// <typeparam name="C">The configuration type.</typeparam>
    public class MicroservicePipeline
    {
        public MicroservicePipeline(IMicroservice service, IEnvironmentConfiguration config)
        {
            if (service == null)
                throw new ArgumentNullException("service cannot be null");
            if (config == null)
                throw new ArgumentNullException("config cannot be null");

            Service = service;
            Configuration = config;
        }

        /// <summary>
        /// This is the service contract.
        /// </summary>
        public IMicroservice Service { get; }

        /// <summary>
        /// This is the external configuration.
        /// </summary>
        public IEnvironmentConfiguration Configuration { get; }

        /// <summary>
        /// This shortcut starts the service.
        /// </summary>
        public void Start()
        {
            Service.Start();
        }

        /// <summary>
        /// This shortcut stops the service.
        /// </summary>
        public void Stop()
        {
            Service.Stop();
        }
    }
}
