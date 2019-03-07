using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
namespace Xigadee
{
    public partial class ServiceHandlerContainer
    {
        /// <summary>
        /// Gets the serialization collection.
        /// </summary>
        public ServiceHandlerCollection<IServiceHandlerSerialization> Serialization { get; }
    }
}
