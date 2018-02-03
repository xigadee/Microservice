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
    public static partial class AzureExtensionMethods
    {
        /// <summary>
        /// Adds an events hub data collector to the pipeline
        /// </summary>
        /// <typeparam name="P">Type of IPipeline</typeparam>
        /// <param name="pipeline">Pipeline</param>
        /// <param name="connectionString">Event Hub Connection String</param>
        /// <param name="entityPath">Event Path</param>
        /// <param name="adjustPolicy">Adjust Policy Action</param>
        /// <param name="resourceProfile">Resource Profile</param>
        /// <param name="encryptionHandler">Encryption Handler</param>
        /// <param name="onCreate">Collector OnCreate Action</param>
        /// <returns></returns>
        public static P AddEventHubsDataCollector<P>(this P pipeline
            , string connectionString = null
            , string entityPath = null
            , Action<EventHubsDataCollectorPolicy> adjustPolicy = null
            , ResourceProfile resourceProfile = null
            , EncryptionHandlerId encryptionHandler = null
            , Action<EventHubsDataCollector> onCreate = null) where P : IPipeline
        {
            var policy = new EventHubsDataCollectorPolicy();

            if (encryptionHandler != null && !pipeline.Service.ServiceHandlers.Encryption.Contains(encryptionHandler.Id))
                throw new EncryptionHandlerNotResolvedException(encryptionHandler.Id);

            adjustPolicy?.Invoke(policy);

            if (connectionString == null)
                connectionString = pipeline.Configuration.EventHubsConnection();

            var component = new EventHubsDataCollector(connectionString
                , entityPath
                , policy
                , resourceProfile
                , encryptionHandler);

            onCreate?.Invoke(component);

            pipeline.AddDataCollector(component);

            return pipeline;
        }
    }
}
