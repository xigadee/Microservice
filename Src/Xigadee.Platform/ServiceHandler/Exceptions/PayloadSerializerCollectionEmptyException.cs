using System;

namespace Xigadee
{
    /// <summary>
    /// This exception is set if no serializer has been set for the Microservice. 
    /// Xigadee needs at least on Serializer to function effectively.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class PayloadSerializerCollectionIsEmptyException:Exception
    {
    }
}
