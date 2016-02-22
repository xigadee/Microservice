using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the base class that is used for serialization payload transmitted
    /// over the API.
    /// </summary>
    public interface ITransportSerializer<E> : ITransportSerializer
    {

        E GetObject(byte[] data, Encoding encoding = null);

        byte[] GetData(E entity, Encoding encoding = null);


    }

    public interface ITransportSerializer
    {
        Type EntityType { get; }

        string MediaType { get; }

        double Priority { get; set; }
    }
}
