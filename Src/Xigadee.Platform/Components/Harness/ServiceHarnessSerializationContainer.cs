using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the stub serializtion container.
    /// </summary>
    public class ServiceHarnessSerializationContainer: SerializationContainer
    {

        protected override void StartInternal()
        {
            Add(new JsonContractSerializer());
            base.StartInternal();
        }
    }
}
