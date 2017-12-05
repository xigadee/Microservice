using System;

namespace Xigadee
{
    /// <summary>
    /// This wrapper is used to reduce the number of security interfaces implemented by the Microservice.
    /// </summary>
    internal class SerializationWrapper: WrapperBase, IMicroserviceSerialization
    {
        /// <summary>
        /// This contains the supported serializers.
        /// </summary>
        SerializationContainer mContainer;

        internal SerializationWrapper(SerializationContainer container, Func<ServiceStatus> getStatus) : base(getStatus)
        {
            mContainer = container;
        }

        //Serializer
        #region RegisterPayloadSerializer...        
        /// <summary>
        /// Registers the payload serializer.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <returns>Returns the serializer.</returns>
        public virtual IPayloadSerializer RegisterPayloadSerializer(IPayloadSerializer serializer)
        {
            ValidateServiceNotStarted();
            return mContainer.Add(serializer);
        }
        /// <summary>
        /// This method allows you to manually register a payload serializer.
        /// </summary>
        /// <param name="fnSerializer">The serializer creation function.</param>
        /// <returns>Returns the new serializer.</returns>
        public virtual IPayloadSerializer RegisterPayloadSerializer(Func<IPayloadSerializer> fnSerializer)
        {
            ValidateServiceNotStarted();
            return mContainer.Add(fnSerializer);
        }            
        #endregion
        #region ClearPayloadSerializers()
        /// <summary>
        /// This method clears the payload serializers. This may be used as by default, we add the JSON based serializer to Microservice.
        /// </summary>
        public virtual void ClearPayloadSerializers()
        {
            ValidateServiceNotStarted();
            mContainer.Clear();
        }
        #endregion

        /// <summary>
        /// Gets the payload serializer count.
        /// </summary>
        public virtual int PayloadSerializerCount => mContainer?.Count ?? 0;
    }
}
