
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This extension allows for specific events to be inspected.
        /// </summary>
        /// <param name="pipeline">The incoming pipeline.</param>
        /// <param name="action">An action to process the event.</param>
        /// <param name="evFilter">A filter function to specify the event types that can be processed.</param>
        /// <returns>The pass-through of the pipeline.</returns>
        public static P OnEvent<P>(this P pipeline
            , Action<EventArgs> action = null
            , Func<EventArgs, bool> evFilter = null)
            where P : IPipeline
        {

            return pipeline;
        }
    }
}
