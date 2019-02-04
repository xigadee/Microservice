using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by Pipeline extensions.
    /// </summary>
    public interface IPipelineExtension<out P>:IPipelineBase
        where P: IPipeline
    {
        /// <summary>
        /// This is the configuration pipeline.
        /// </summary>
        P Pipeline { get; }
    }
}
