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

using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Xigadee
{
    /// <summary>
    /// This collector is used to connect Xigadee event logging to Azure EventHubs fabric.
    /// </summary>
    public class EventHubsDataCollector: DataCollectorBase<DataCollectorStatistics, EventHubsDataCollectorPolicy>
    {
        protected EventHubClient mEventHubClient;

        protected readonly string mConnection;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection">Connection string to Event Hub including the entity path</param>
        /// <param name="entityPath">Entity path if not supplied in the connection string</param>
        /// <param name="policy">Policy</param>
        /// <param name="resourceProfile">Resource Profile</param>
        /// <param name="encryptionId">Encryption Id</param>
        /// <param name="supportMap">Support Map</param>
        public EventHubsDataCollector(string connection
            , string entityPath
            , EventHubsDataCollectorPolicy policy = null
            , ResourceProfile resourceProfile = null
            , EncryptionHandlerId encryptionId = null
            , DataCollectionSupport? supportMap = null) : base(encryptionId, resourceProfile, supportMap, policy)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(connection);
            if (!string.IsNullOrWhiteSpace(entityPath))
                connectionStringBuilder.EntityPath = entityPath;

            mConnection = connectionStringBuilder.ToString();
        }

        #region Start / Stop
        /// <summary>
        /// Start Internal
        /// </summary>
        protected override void StartInternal()
        {
            base.StartInternal();
            mEventHubClient = CreateEventHubClient();
        }

        /// <summary>
        /// Stop Internal
        /// </summary>
        protected override void StopInternal()
        {
            mEventHubClient.Close();
            base.StopInternal();
        }
        #endregion

        #region SupportLoadDefault()
        /// <summary>
        /// This maps the default support for the event types.
        /// </summary>
        protected override void SupportLoadDefault()
        {
            SupportAdd(DataCollectionSupport.EventSource, (e) => WriteEvent(DataCollectionSupport.EventSource, e));
        }
        #endregion

        /// <summary>
        /// Creates a new event hub client
        /// </summary>
        protected EventHubClient CreateEventHubClient()
        {
            var client = EventHubClient.CreateFromConnectionString(mConnection);
            client.RetryPolicy = new RetryExponential(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), 5);
            return client;
        }

        /// <summary>
        /// Write event source 
        /// </summary>
        /// <param name="support"></param>
        /// <param name="eventHolder"></param>
        protected virtual void WriteEvent(DataCollectionSupport support, EventHolder eventHolder)
        {
            var options = mPolicy.Options.FirstOrDefault(o => o.Support == support);
            if (options == null || !options.IsSupported(eventHolder))
                return;

            int start = StatisticsInternal.ActiveIncrement(options.Support);
            Guid? traceId = options.ShouldProfile ? (ProfileStart($"AzureEventHub{options.Support}_{eventHolder.Data.TraceId}")) : default(Guid?);

            // Check we have en event hub client
            mEventHubClient = mEventHubClient ?? CreateEventHubClient();

            var result = ResourceRequestResult.Unknown;
            try
            {
                var blob = options.SerializerBinary(eventHolder, OriginatorId).Blob;
                if (options.EncryptionPolicy != AzureStorageEncryption.None && mEncryption != null)
                    blob = (Security?.Encrypt(mEncryption, blob) ?? blob);

                mEventHubClient.SendAsync(new EventData(blob)).Wait();
                result = ResourceRequestResult.Success;
            }
            catch (Exception)
            {
                result = ResourceRequestResult.Exception;
                StatisticsInternal.ErrorIncrement(options.Support);
                throw;
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(options.Support, start);
                if (traceId.HasValue)
                    ProfileEnd(traceId.Value, start, result);
            }
        }
    }
}
