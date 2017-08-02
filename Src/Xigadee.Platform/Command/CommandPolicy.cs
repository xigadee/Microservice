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
    /// The command policy sets or enables various settings for the command.
    /// </summary>
    public class CommandPolicy:PolicyBase
    {
        /// <summary>
        /// Gets or sets the master job negotiation strategy.
        /// </summary>
        public MasterJobNegotiationStrategyBase MasterJobNegotiationStrategy { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the TransmissionPayload trace flag should be set to true.
        /// </summary>
        public bool TransmissionPayloadTraceEnabled { get; set; }
        /// <summary>
        /// This is the default listening channel for incoming messages.
        /// </summary>
        public virtual string ChannelId { get; set; }
        /// <summary>
        /// This property specifies that the listening channel id can be automatically set during configuration.
        /// </summary>
        public bool ChannelIdAutoSet { get; set; } = true;
        /// <summary>
        /// This is the response channel set in outgoing request, and is used to route response messages back to the command.
        /// </summary>
        public virtual string ResponseChannelId { get; set; }
        /// <summary>
        /// This property specifies whether the response channel can be automatically set by the pipeline.
        /// </summary>
        public bool ResponseChannelIdAutoSet { get; set; } = true;

        /// <summary>
        /// This is the channel for incoming masterjob negotiation messages.
        /// </summary>
        public virtual string MasterJobNegotiationChannelIdIncoming { get; set; }
        /// <summary>
        /// This is the channel for outgoing masterjob negotiation messages, if this is null then the incoming channel will be used.
        /// </summary>
        public virtual string MasterJobNegotiationChannelIdOutgoing { get; set; }
        /// <summary>
        /// This property specifies whether these channels can be set automatically from the configuration pipeline.
        /// </summary>
        public bool MasterJobNegotiationChannelIdAutoSet { get; set; } = true;

        /// <summary>
        /// This is the frequency that jobs wait between polling for status. The default is 20s.
        /// </summary>
        public TimeSpan? MasterJobPollFrequency { get; set; } = TimeSpan.FromSeconds(20);
        /// <summary>
        /// This is the initial wait after a master job starts that it waits to begin polling. The default is 5s.
        /// </summary>
        public TimeSpan? MasterJobPollInitialWait { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// This is the command startup prioroty.
        /// </summary>
        public int? StartupPriority { get; set; }=0;
        /// <summary>
        /// This property specifies how the command notifies the communication container when a new command is registered and becomes active.
        /// </summary>
        public CommandNotificationBehaviour CommandNotify { get; set; } = CommandNotificationBehaviour.OnRegistration;
        /// <summary>
        /// This is the default timeout for outgoing requests from the Command to other commands when not set in the settings.
        /// The default is 30s.
        /// </summary>
        public virtual TimeSpan OutgoingRequestMaxProcessingTimeDefault { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// This boolean property specifies whether outgoing requests are enables.
        /// </summary>
        public virtual bool OutgoingRequestsEnabled { get; set; } = false;
        /// <summary>
        /// This is the default time out poll, which is set at an initial 10 second wait and then a repeated 5 seconds poll by default.
        /// </summary>
        public virtual CommandTimerPoll OutgoingRequestsTimeoutPoll { get; set; } = new CommandTimerPoll() { Interval = TimeSpan.FromSeconds(1) };

        //Job Poll
        public virtual bool JobPollEnabled { get; set; } = false;

        public virtual CommandTimerPoll JobPollSchedule { get; set; } = new CommandTimerPoll();

        public virtual bool JobPollIsLongRunning { get; set; } = false;

        //Master Job
        public virtual bool MasterJobEnabled { get; set; }

        public virtual int MasterJobNegotiationChannelPriority { get; set; } = 2;

        public virtual string MasterJobNegotiationChannelType { get; set; }

        public virtual string MasterJobName { get; set; }


        public static CommandPolicy ToJob(TimeSpan? interval, TimeSpan? initialWait, DateTime? initialTime, bool isLongRunningJob = false)
        {
            return ToJob<CommandPolicy>(interval, initialWait, initialTime, isLongRunningJob);
        }

        public static P ToJob<P>(TimeSpan? interval, TimeSpan? initialWait, DateTime? initialTime, bool isLongRunningJob = false)
            where P : CommandPolicy, new()
        {
            return new P
            {
                  JobPollEnabled = true
                , JobPollIsLongRunning = isLongRunningJob
                , MasterJobEnabled = false
                , JobPollSchedule = new CommandTimerPoll(interval, initialWait, initialTime)
            };
        }

        public static CommandPolicy ToMasterJob(string negotiationChannelId, string negotiationChannelType = null, int negotiationChannelPriority = 1, string name = null)
        {
            return ToMasterJob<CommandPolicy>(negotiationChannelId, negotiationChannelType, negotiationChannelPriority, name);
        }

        public static P ToMasterJob<P>(string negotiationChannelId, string negotiationChannelType = null, int negotiationChannelPriority = 1, string name = null)
            where P: CommandPolicy, new()
        {
            return new P()
            {
                  JobPollEnabled = true
                , MasterJobEnabled = true
                , MasterJobNegotiationChannelIdOutgoing = negotiationChannelId
                , MasterJobNegotiationChannelType = negotiationChannelType
                , MasterJobNegotiationChannelPriority = negotiationChannelPriority
                , MasterJobName = name
            };
        }

        /// <summary>
        /// Specifies that the commands are supported by reflection.
        /// </summary>
        public bool CommandReflectionSupported { get; set; } = true;

        /// <summary>
        /// Specifies that the master job commands are supported by reflection.
        /// </summary>
        public bool CommandMasterJobReflectionSupported { get; set; } = true;

        /// <summary>
        /// Specifies that schedule reflection commands are supported.
        /// </summary>
        public bool CommandScheduleReflectionSupported { get; set; } = true;
    }
}
