//using System;

//namespace Xigadee
//{
//    /// <summary>
//    /// This wrapper is used to reduce the number of security interfaces implemented by the Microservice.
//    /// </summary>
//    internal class SerializationWrapper: WrapperBase, IMicroserviceSerialization
//    {
//        /// <summary>
//        /// This contains the supported serializers.
//        /// </summary>
//        SerializationContainer mContainer;

//        internal SerializationWrapper(SerializationContainer container, Func<ServiceStatus> getStatus) : base(getStatus)
//        {
//            mContainer = container;
//        }

//        //Serializer
//        #region RegisterPayloadSerializer...        
//        /// <summary>
//        /// Registers the payload serializer.
//        /// </summary>
//        /// <param name="serializer">The serializer.</param>
//        /// <returns>Returns the serializer.</returns>
//        public virtual ISerializationSerializer RegisterPayloadSerializer(ISerializationSerializer serializer)
//        {
//            ValidateServiceNotStarted();
//            return mContainer.Add(serializer);
//        }
//        /// <summary>
//        /// This method allows you to manually register a payload serializer.
//        /// </summary>
//        /// <param name="fnSerializer">The serializer creation function.</param>
//        /// <returns>Returns the new serializer.</returns>
//        public virtual ISerializationSerializer RegisterPayloadSerializer(Func<ISerializationSerializer> fnSerializer)
//        {
//            ValidateServiceNotStarted();
//            return mContainer.Add(fnSerializer());
//        }            
//        #endregion
//        #region ClearPayloadSerializers()
//        /// <summary>
//        /// This method clears the payload serializers. This may be used as by default, we add the JSON based serializer to Microservice.
//        /// </summary>
//        public virtual void ClearPayloadSerializers()
//        {
//            ValidateServiceNotStarted();
//            mContainer.ClearSerializers();
//        }
//        #endregion
//        /// <summary>
//        /// Gets the payload serializer count.
//        /// </summary>
//        public int PayloadSerializerCount => mContainer?.CountSerializers ?? 0;

//        #region RegisterPayloadCompressor ...
//        /// <summary>
//        /// Registers the payload compressor.
//        /// </summary>
//        /// <param name="compressor">The compressor creation function.</param>
//        /// <returns>
//        /// Returns the compressor.
//        /// </returns>
//        public ISerializationCompressor RegisterPayloadCompressor(Func<ISerializationCompressor> compressor)
//        {
//            ValidateServiceNotStarted();
//            return mContainer.Add(compressor());
//        }
//        /// <summary>
//        /// Registers the payload compressor.
//        /// </summary>
//        /// <param name="compressor">The compressor.</param>
//        /// <returns>
//        /// Returns the compressor.
//        /// </returns>
//        public ISerializationCompressor RegisterPayloadCompressor(ISerializationCompressor compressor)
//        {
//            ValidateServiceNotStarted();
//            return mContainer.Add(compressor);
//        }
//        #endregion
//        #region ClearPayloadCompressors()
//        /// <summary>
//        /// Clears the payload compressors.
//        /// </summary>
//        public void ClearPayloadCompressors()
//        {
//            ValidateServiceNotStarted();
//            mContainer.ClearCompressors();
//        } 
//        #endregion
//        /// <summary>
//        /// Gets the payload compressor count.
//        /// </summary>
//        public int PayloadCompressorCount => mContainer?.CountCompressors ?? 0;
//    }
//}
