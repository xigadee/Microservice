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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the client and associates statistics. 
    /// This also ensures that the client can only have one active poll.
    /// </summary>
    public class ClientPriorityHolder:StatisticsBase<ClientPriorityHolderStatistics>
    {
        #region Declarations
        /// <summary>
        /// This is the private reserve lock used to ensure concurrency when setting the IsActive flag.
        /// </summary>
        private object mReserveLock = new object();

        private string mMappingChannel;

        private ClientPriorityHolderMetrics mMetrics;

        private IListenerClientPollAlgorithm mPriorityAlgorithm;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the main constructor.
        /// </summary>
        /// <param name="resourceTracker">The resource tracker.</param>
        /// <param name="client">The client to hold.</param>
        /// <param name="mappingChannelId">The mapping channel.</param>
        /// <param name="maxAllowedPollWait">The maximum permitted poll length.</param>
        public ClientPriorityHolder(IResourceTracker resourceTracker
            , ClientHolder client
            , string mappingChannelId
            , IListenerClientPollAlgorithm priorityAlgorithm
            )
        {
            if (client == null)
                throw new ArgumentNullException("client");

            if (priorityAlgorithm == null)
                throw new ArgumentNullException("algorithm");

            mPriorityAlgorithm = priorityAlgorithm;

            Client = client;

            mMappingChannel = mappingChannelId;

            //Create the metrics container to hold the calculations for poll priority and reservation amount.
            mMetrics = new ClientPriorityHolderMetrics(mPriorityAlgorithm
                , resourceTracker?.RegisterRequestRateLimiter(client.Name, client.ResourceProfiles)
                , client.Priority
                , client.Weighting
                );

            mPriorityAlgorithm.InitialiseMetrics(mMetrics);
        }
        #endregion

        #region StatisticsRecalculate()
        /// <summary>
        /// This method calculates the statistics for the client.
        /// </summary>
        protected override void StatisticsRecalculate(ClientPriorityHolderStatistics stats)
        {
            stats.Id = Id;
            stats.ClientId = Client.Id;       

            stats.Algorithm = mPriorityAlgorithm.Name;
            stats.Name = Name;
            stats.MappingChannel = mMappingChannel;
            
            stats.IsReserved = IsReserved;
            stats.Reserved = Reserved;
            
            stats.LastException = LastException;
            stats.LastExceptionTime = LastExceptionTime;
            
            stats.Metrics = mMetrics.Statistics;
        }
        #endregion

        #region Id
        /// <summary>
        /// This is the unique reference id for the holder.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid(); 
        #endregion
        #region Name
        /// <summary>
        /// This is the friendly name.
        /// </summary>
        public string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(Client.MappingChannelId))
                    return $"{Client.Name}-{Client.MappingChannelId}";
                else
                    return Client.Name;
            }
        }
        #endregion

        #region IsPollPastDue
        /// <summary>
        /// A shortcut that identifies whether the client is a deadletter
        /// </summary>
        public bool IsPollPastDue
        {
            get
            {
                return mMetrics.IsPollPastDue;
            }
        }
        #endregion
        #region Client
        /// <summary>
        /// This is the underlying client.
        /// </summary>
        protected ClientHolder Client { get; private set; }
        #endregion
        #region ClientId
        /// <summary>
        /// This is the same id as the underlying client.
        /// </summary>
        public Guid ClientId
        {
            get
            {
                return Client.Id;
            }
        }
        #endregion
        #region IsActive
        /// <summary>
        /// A shortcut that defines whether the client is active
        /// </summary>
        public bool IsActive
        {
            get
            {
                return Client.IsActive;
            }
        } 
        #endregion

        #region --> IsReserved
        /// <summary>
        /// This boolean property identifies whether the client is currently reserved and active.
        /// </summary>
        public bool IsReserved { get; private set; }
        #endregion
        #region Reserved
        /// <summary>
        /// This is the last reserved slot value for the client.
        /// </summary>
        public int? Reserved { get; set; }
        #endregion

        #region ShouldSkip()
        /// <summary>
        /// This method specifies whether the client holder should be skipped for the current poll.
        /// </summary>
        /// <returns>Returns true if it should be skipped.</returns>
        public bool ShouldSkip()
        {
            return mMetrics.ShouldSkip();
        } 
        #endregion

        #region Reserve(int available, out int taken)
        /// <summary>
        /// This method reserves the client, and returns the number of slots that it has taken based on
        /// previous history.
        /// </summary>
        /// <param name="available">The available capacity.</param>
        /// <param name="taken">The number of slots actually reserved.</param>
        /// <returns>Returns true if the holder is reserved for polling.</returns>
        public bool Reserve(int available, out int taken)
        {
            taken = 0;

            if (!IsActive)
                return false;

            lock (mReserveLock)
            {
                //Let's set thread safe barrier.
                if (IsReserved)
                    return false;

                //Ok, set a barrier to stop other processes getting in.
                IsReserved = true;
            }

            //Check that we won the interlocked battle
            //There are multiple threads running around here, so it pays to be paranoid.
            if (IsReserved)
            {
                int takenCalc = mMetrics.Reserve(available);

                if (takenCalc > 0 && IsActive)
                {
                    Reserved = taken = takenCalc;
                    return true;
                }
                else
                {
                    //Ok, we won the battle, but nothing to do here.
                    Release(false);
                }
            }

            return false;
        }
        #endregion
        #region Poll()
        /// <summary>
        /// This method is used to Poll the connection for waiting messages.
        /// </summary>
        /// <returns>Returns a list of TransmissionPayload objects to process.</returns>
        public async Task<List<TransmissionPayload>> Poll()
        {
            List<TransmissionPayload> payloads = null;
            bool hasErrored = false;

            try
            {
                int? fabricWaitTimeInMs = mMetrics.PollBegin(Reserved.Value);
                payloads = await Client.MessagesPull(Reserved.Value, fabricWaitTimeInMs, mMappingChannel);

            }
            catch (Exception ex)
            {
                LastException = ex;
                LastExceptionTime = DateTime.UtcNow;
                hasErrored = true;
            }
            finally
            {
                mMetrics.PollEnd(payloads?.Count ?? 0, hasErrored);
            }

            return payloads;
        }
        #endregion
        #region Release(bool exception)
        /// <summary>
        /// This method releases the holder so that it can be polled again.
        /// </summary>
        /// <param name="exception">A flag indicating whether there was an exception.</param>
        /// <returns>Returns true if the holder returned all records requested.</returns>
        public void Release(bool exception)
        {
            lock (mReserveLock)
            {
                mMetrics.Release(exception);

                IsReserved = false;
            }
        }
        #endregion

        #region Priority
        /// <summary>
        /// This is the current client priority.
        /// </summary>
        public int Priority { get { return mMetrics.Priority; } }
        #endregion
        #region PriorityWeighting
        /// <summary>
        /// This is the client weighting which is used to adjust the priority for the polling.
        /// This value is a percentage ratio.
        /// </summary>
        public decimal PriorityWeighting { get { return mMetrics.PriorityWeighting; } }
        #endregion

        #region PriorityRecalculate()
        /// <summary>
        /// This method polls the client for the queue length and resets the poll priority.
        /// </summary>
        /// <returns>Returns the new priority.</returns>
        public long PriorityRecalculate()
        {
            long? queueLength = null;
            try
            {
                queueLength = Client.QueueLength();
            }
            catch (Exception ex)
            {
                //We don't want to throw an error here
            }

            return mMetrics.PriorityRecalculate(queueLength);
        } 
        #endregion
        #region CapacityReset()
        /// <summary>
        /// This method sets the capicity statistics to the calculated defaults.
        /// </summary>
        public void CapacityReset()
        {
            mMetrics.CapacityReset();
        } 
        #endregion

        #region LastException
        /// <summary>
        /// This is the last recorded exception that occurred during polling
        /// </summary>
        public Exception LastException { get; set; }
        /// <summary>
        /// This is the last time for a poll exception.
        /// </summary>
        public DateTime? LastExceptionTime { get; set; }
        #endregion

        #region Logging
        public void QueueTimeLog(DateTime? EnqueuedTimeUTC)
        {
            Client.QueueTimeLog(EnqueuedTimeUTC);
        }

        public void ActiveIncrement()
        {
            Client.ActiveIncrement();
        }

        public void ActiveDecrement(int TickCount)
        {
            Client.ActiveDecrement(TickCount);
        }

        public void ErrorIncrement()
        {
            Client.ErrorIncrement();
        }
        #endregion
    }
}
