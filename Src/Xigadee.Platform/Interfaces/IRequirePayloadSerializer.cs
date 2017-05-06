using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is for components that require payload serialization and deserialization.
    /// </summary>
    public interface IRequirePayloadSerializer
    {
        /// <summary>
        /// This is the system wide Payload serializer.
        /// </summary>
        IPayloadSerializationContainer PayloadSerializer { get; set; }
    }
}
