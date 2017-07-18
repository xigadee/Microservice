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

#region using
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the generic base class that is used by the TaskManager to abstract fabric specific 
    /// implementations away from the task scheduler code.
    /// </summary>
    [DebuggerDisplay("{Type}|{Name}|{Priority} Active={IsActive} {Id}")]
    public abstract class ClientHolder: StatisticsBase<MessagingServiceStatistics>, IRequireDataCollector
    {
        #region Declarations
        /// <summary>
        /// This is the unique client id.
        /// </summary>
        public readonly Guid Id = Guid.NewGuid();
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor for the client.
        /// </summary>
        public ClientHolder()
        {
            MaxRetries = 5;
            Priority = 1;
            CanStart = true;
            LastTickCount = Environment.TickCount;
            QueueLength = () => (int?)null;
            Filters = new List<string>();
        }
        #endregion

        #region Logging
        /// <summary>
        /// This method logs the last time a message was enqueued.
        /// </summary>
        /// <param name="EnqueuedTimeUTC">The current UTC time.</param>
        public void QueueTimeLog(DateTime? EnqueuedTimeUTC)
        {
            StatisticsInternal.QueueTimeLog(EnqueuedTimeUTC);
            StatisticsInternal.ActiveIncrement();
        }

        /// <summary>
        /// This method decrements the active messages.
        /// </summary>
        /// <param name="start">The tick count when the process started.</param>
        public void ActiveDecrement(int start)
        {
            StatisticsInternal.ActiveDecrement(start);
        }
        /// <summary>
        /// This method increments the error count.
        /// </summary>
        public void ErrorIncrement()
        {
            StatisticsInternal.ErrorIncrement();
        }
        #endregion

        /// <summary>
        /// This method pulls fabric messages and converts them in to generic payload messages for the Microservice to process.
        /// </summary>
        /// <param name="count">The maximum number of messages to return.</param>
        /// <param name="wait">The maximum wait in milliseconds</param>
        /// <param name="mappingChannel">This is the incoming mapping channel for subscription based client where the subscription maps
        /// to a new incoming channel on the same topic.</param>
        /// <returns>Returns a list of transmission for processing.</returns>
        public abstract Task<List<TransmissionPayload>> MessagesPull(int? count, int? wait, string mappingChannel = null);

        /// <summary>
        /// This method is used to Transmit the payload. You should override this method to insert your own tranmission logic.
        /// </summary>
        /// <param name="payload">The payload to transmit.</param>
        /// <param name="retry">This parameter specifies the number of retries that should be attempted if transmission fails. By default this value is 0.</param>
        /// <returns></returns>
        public abstract Task Transmit(TransmissionPayload payload, int retry = 0);

        /// <summary>
        /// This boolean property idemtifies whether the client is active and should be polled
        /// for new arriving messages.
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// This is the last recorded tick count when the client was active.
        /// </summary>
        public int LastTickCount { get; protected set; }
        /// <summary>
        /// This is the human readable client type.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// This is the actual name of the client used by the underlying architecture.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// This is the internal mapping channel id.
        /// </summary>
        public string MappingChannelId { get; set; }
        /// <summary>
        /// This is the channelId.
        /// </summary>
        public string ChannelId { get; set; }
        /// <summary>
        /// This is the client message priority.
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// This property determines whether the listener can start.
        /// </summary>
        public bool CanStart { get; set; }
        /// <summary>
        /// This action starts the client.
        /// </summary>
        public Action Start { get; set; }
        /// <summary>
        /// This action stops the client.
        /// </summary>
        public Action Stop { get; set; }

        /// <summary>
        /// This method is used to close the client.
        /// </summary>
        public Action ClientClose { get; set; }
        /// <summary>
        /// This method is used to reset the client.
        /// </summary>
        public Action<Exception> ClientReset { get; set; }
        /// <summary>
        /// This method is used to initialise or reinitialise the underlying fabric.
        /// </summary>
        public Action FabricInitialize { get; set; }
        /// <summary>
        /// This is the maximum number of retries to send the message.
        /// </summary>
        public int MaxRetries { get; set; }
        /// <summary>
        /// This method returns the length on the queue if supported, null if not supported.
        /// </summary>
        public Func<long?> QueueLength { get; set; }
        /// <summary>
        /// This is the last time the queue length was retrieved.
        /// </summary>
        public DateTime? QueueLengthLastPoll { get; set; }
        /// <summary>
        /// This boolean property identifies if the client supports a queue length poll
        /// </summary>
        public bool SupportsQueueLength { get; set; }
        /// <summary>
        /// This property indicates whether the client will dynamically rate limit its request rate when signalled by the microservice resource handlers.
        /// </summary>
        public bool SupportsRateLimiting { get; set; }
        /// <summary>
        /// This property allows a channel to receive a higer poll frequency by setting the rating greater than 1.
        /// </summary>
        public decimal Weighting { get; set; }
        /// <summary>
        /// This is the list of filters.
        /// </summary>
        public List<string> Filters { get; set; }
        /// <summary>
        /// This is the maximum allowable processing time for a message.
        /// </summary>
        public TimeSpan? MessageMaxProcessingTime { get; set; }
        /// <summary>
        /// This is the maximum wait time for the underlying fabric to hold the message.
        /// </summary>
        public TimeSpan? FabricMaxMessageLock { get; set; }
        /// <summary>
        /// This method is used to validate the current filter settings for the client.
        /// The default is to do nothing.
        /// </summary>
        public Action ClientRefresh;

        #region Logger/LogException
        /// <summary>
        /// This shortcut logs to the collector for log messaging
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The exception.</param>
        protected void LogException(string message, Exception ex)
        {
            Collector?.LogException(string.Format("{0}={1} - {2}", Name, message, ex.Message), ex);
        }
        #endregion

        #region Collector
        /// <summary>
        /// This is the system wide data collector
        /// </summary>
        public IDataCollection Collector{get; set;}
        #endregion
        #region BoundaryLoggingActive
        /// <summary>
        /// This boolean property specifies whether boundary logging is active for this client.
        /// The default is false.
        /// </summary>
        public bool BoundaryLoggingActive { get; set; } 
        #endregion

        #region StatisticsRecalculate()
        /// <summary>
        /// This method recalculates the statistics for the client.
        /// </summary>
        protected override void StatisticsRecalculate(MessagingServiceStatistics stats)
        {
            stats.Name = DebugStatus;

            stats.QueueLength = QueueLength();
            stats.Filters = Filters;
            stats.IsActive = IsActive;
            stats.Id = this.Id;
        }
        #endregion

        /// <summary>
        /// This contains the listener resource profiles.
        /// </summary>
        public List<ResourceProfile> ResourceProfiles { get; set; }

        /// <summary>
        /// This is the client status.
        /// </summary>
        public string DebugStatus { get { return string.Format("{0}: {1} [{2}] ({3}) {4}", Type, Name, Priority, IsActive ? "Active" : "Inactive", Id); } }
    }
}
