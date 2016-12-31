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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public SerializationWrapper(SerializationContainer container, Func<ServiceStatus> getStatus) : base(getStatus)
        {
            mContainer = container;
        }

        //Serializer
        #region RegisterPayloadSerializer(IPayloadSerializer serializer)
        /// <summary>
        /// This method allows you to manually register a requestPayload serializer.
        /// </summary>
        /// <typeparam name="C">The requestPayload serializer channelId.</typeparam>
        public virtual IPayloadSerializer RegisterPayloadSerializer(IPayloadSerializer serializer)
        {
            ValidateServiceNotStarted();
            mContainer.Add(serializer);
            return serializer;
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

    }
}
