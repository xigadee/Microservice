#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
namespace Xigadee
{
    /// <summary>
    /// This interface is used to expose the serialization container to applications 
    /// that require access to it.
    /// </summary>
    public interface IPayloadSerializationContainer
    {
        /// <summary>
        /// This method extracts the binary blob from the message and deserializes and returns the object.
        /// </summary>
        /// <param name="payload">The transmission payload.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        object PayloadDeserialize(TransmissionPayload payload);

        /// <summary>
        /// This method tries to extract an entity from the message if present.
        /// </summary>
        /// <param name="payload">The transmission payload.</param>
        /// <param name="entity">The deserialized entity</param>
        /// <returns>Returns true if the message is present.</returns>
        bool PayloadTryDeserialize(TransmissionPayload payload, out object entity);

        /// <summary>
        /// This method extracts the binary blob from the message and deserializes and returns the object.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="payload">The transmission payload.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        P PayloadDeserialize<P>(TransmissionPayload payload);

        /// <summary>
        /// This method tries to extract an entity from the message if present.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="payload">The transmission payload.</param>
        /// <param name="entity">The deserialized entity</param>
        /// <returns>Returns true if the message is present.</returns>
        bool PayloadTryDeserialize<P>(TransmissionPayload payload, out P entity);

        /// <summary>
        /// This method extracts the binary blob from the message and deserializes and returns the object.
        /// </summary>
        /// <param name="message">The service message.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        object PayloadDeserialize(ServiceMessage message);

        /// <summary>
        /// This method tries to extract an entity from the message if present.
        /// </summary>
        /// <param name="message">The service message.</param>
        /// <param name="entity">The deserialized entity</param>
        /// <returns>Returns true if the message is present.</returns>
        bool PayloadTryDeserialize(ServiceMessage message, out object entity);

        /// <summary>
        /// This method extracts the binary blob from the message and deserializes and returns the object.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="message">The service message.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        P PayloadDeserialize<P>(ServiceMessage message);

        /// <summary>
        /// This method tries to extract an entity from the message if present.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="message">The service message.</param>
        /// <param name="entity">The deserialized entity</param>
        /// <returns>Returns true if the message is present.</returns>
        bool PayloadTryDeserialize<P>(ServiceMessage message, out P entity);

        /// <summary>
        /// This method deserializes the binary blob and returns the object.
        /// </summary>
        /// <param name="blob">The binary blob.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        object PayloadDeserialize(byte[] blob);

        /// <summary>
        /// This method deserializes the binary blob and returns the object.
        /// </summary>
        /// <typeparam name="P">The payload message type.</typeparam>
        /// <param name="blob">The binary blob.</param>
        /// <returns>Returns the object deserialized from the binary blob.</returns>
        P PayloadDeserialize<P>(byte[] blob);

        /// <summary>
        /// This method serializes the requestPayload object in to a binary blob using the 
        /// serializer collection.
        /// </summary>
        /// <param name="payload">The requestPayload to serialize.</param>
        /// <returns>Returns the binary blob object.</returns>
        byte[] PayloadSerialize(object payload);
    }
}
