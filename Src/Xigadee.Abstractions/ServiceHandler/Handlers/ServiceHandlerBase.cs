using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the base abstract class that service handler components are built upon.
    /// </summary>
    public abstract class ServiceHandlerBase : IServiceHandler
    {

        /// <summary>
        /// Gets the content-type parameter, which can be used to quickly identify the serialization type used.
        /// </summary>
        public virtual string Id { get; protected set; }
        /// <summary>
        /// Gets the friendly name.
        /// </summary>
        public virtual string Name { get; protected set; }
    }
}
