using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to hold the pipeline configuration.
    /// </summary>
    public class SecurityPipelineExtension<P>: MicroservicePipelineExtension<P>, IPipelineSecurity<P>
        where P : IPipeline
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        public SecurityPipelineExtension(P pipeline) : base(pipeline)
        {
        }


    }
}
