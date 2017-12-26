using System;
using System.Collections.Generic;

namespace Xigadee
{
    //SerializationHolder

    public interface IPayloadSerializerSerializationHolder
    {
        bool SupportsObjectTypeSerialization(SerializationHolder holder);

        bool SupportsPayloadDeserialization(SerializationHolder holder);

        bool TryDeserialize(SerializationHolder holder);

        bool TrySerialize(SerializationHolder holder);
    }
}
