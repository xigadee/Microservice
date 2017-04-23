#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the interface used to serialize and deserialize payloads.
    /// </summary>
    public interface IPayloadSerializer: IDisposable
    {
        IEnumerable<byte[]> PayloadMagicNumbers();

        bool SupportsObjectTypeSerialization(Type entityType);

        bool SupportsPayloadDeserialization(byte[] blob);

        bool SupportsPayloadDeserialization(byte[] blob, int start, int length);

        E Deserialize<E>(byte[] blob);

        E Deserialize<E>(byte[] blob, int start, int length);

        object Deserialize(byte[] blob);

        object Deserialize(byte[] blob, int start, int length);

        byte[] Serialize<E>(E entity);

        byte[] Serialize(object entity);

    }
}
