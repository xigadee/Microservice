using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is the pipeline setup helper.
    /// </summary>
    public static class MicroservicePipelineHelper
    {
        /// <summary>
        /// This is the base setting.
        /// </summary>
        /// <param name="service">This is the service.</param>
        /// <param name="config">This is the configuration settings.</param>
        /// <returns>Returns the pipeline object.</returns>
        public static MicroservicePipeline ToPipeline(this IMicroservice service, IEnvironmentConfiguration config)
        {
            return new MicroservicePipeline(service, config);
        }
    }
}
