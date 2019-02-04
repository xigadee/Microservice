using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the generic channel interface
    /// </summary>
    public interface IPipelineSecurity<out P>: IPipelineSecurity, IPipelineExtension<P>
        where P : IPipeline
    {
    }

    public interface IPipelineSecurity
    {

    }

}
