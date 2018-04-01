using System;

namespace Xigadee
{
    /// <summary>
    /// THis exception is thrown when a duplicate service handler is added to a collection
    /// </summary>
    public class ServiceHandlerDuplicationException:Exception
    {
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="id">The handler id.</param>
        /// <param name="handlerType">The handler type.</param>
        /// <param name="collectionType">This is the handler collection type.</param>
        public ServiceHandlerDuplicationException(string id, string handlerType, string collectionType)
            :base($"Duplicate exception for {collectionType}: '{handlerType}'='{id}'. The handler has already been added.")
        {
                
        }
    }
}
