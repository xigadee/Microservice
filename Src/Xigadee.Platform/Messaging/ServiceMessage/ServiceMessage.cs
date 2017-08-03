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
using System.Diagnostics;
using System.Runtime.Serialization; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// The ServiceMessage is used to pass information through the Dispatcher and the sender/receive queue architecture.
    /// </summary>
    [Serializable]
    [DataContract]
    [DebuggerDisplay("Type={ChannelId}/{MessageType}/{ActionType} {ResponseChannelId} [{OriginatorServiceId}]")]
    public class ServiceMessage
    {
        #region Constructor
        /// <summary>
        /// This is the empty constructor.
        /// </summary>
        public ServiceMessage()
        {
            OriginatorKey = Guid.NewGuid().ToString("N").ToUpperInvariant();
            OriginatorUTC = DateTime.UtcNow;
        }
        /// <summary>
        /// This constructor sets the header parameters.
        /// </summary>
        /// <param name="header">The header</param>
        public ServiceMessage(ServiceMessageHeader header):this()
        {
            ChannelId = header.ChannelId;
            MessageType = header.MessageType;
            ActionType = header.ActionType;
        }
        #endregion

        #region SecuritySignature
        /// <summary>
        /// This is the security signature.
        /// </summary>
        [DataMember]
        public string SecuritySignature { get; set; }
        #endregion

        #region OriginatorKey
        /// <summary>
        /// This is the unique key assigned by the system.
        /// </summary>
        [DataMember]
        public string OriginatorKey { get; set; } 
        #endregion
        #region OriginatorServiceId
        /// <summary>
        /// The name of the machine that created the message.
        /// </summary>
        [DataMember]
        public string OriginatorServiceId { get; set; } 
        #endregion
        #region OriginatorUTC
        /// <summary>
        /// Gets or sets the time stamp of the message originator in UTC.
        /// </summary>
        [DataMember]
        public DateTime OriginatorUTC { get; set; }
        #endregion

        #region ProcessCorrelationKey
        /// <summary>
        /// Gets or sets the correlation key which is the original key reference.
        /// </summary>
        [DataMember]
        public string ProcessCorrelationKey { get; set; }
        #endregion

        #region CorrelationKey
        /// <summary>
        /// Gets or sets the correlation key which is the original key reference.
        /// </summary>
        [DataMember]
        public string CorrelationKey { get; set; } 
        #endregion
        #region CorrelationServiceId
        /// <summary>
        /// This is the Id of the service that created the message.
        /// </summary>
        [DataMember]
        public string CorrelationServiceId { get; set; }
        #endregion
        #region CorrelationUTC
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        [DataMember]
        public DateTime? CorrelationUTC { get; set; }
        #endregion

        #region EnqueuedTimeUTC
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        public DateTime? EnqueuedTimeUTC { get; set; }
        #endregion

        #region DispatcherTransitCount
        /// <summary>
        /// This message sets the dispatcher transit hop count.
        /// </summary>
        [DataMember]
        public int DispatcherTransitCount { get; set; } 
        #endregion

        #region ChannelId
        /// <summary>
        /// Gets or sets the channelId of the request message.
        /// </summary>
        [DataMember]
        public string ChannelId { get; set; } 
        #endregion
        #region ChannelPriority
        /// <summary>
        /// Gets or sets the priority for the ChannelId.
        /// </summary>
        [DataMember]
        public int ChannelPriority { get; set; }
        #endregion
        #region MessageType
        /// <summary>
        /// Gets or sets the channelId.
        /// </summary>
        [DataMember]
        public string MessageType { get; set; } 
        #endregion
        #region ActionType
        /// <summary>
        /// Gets or sets the channelId of the action.
        /// </summary>
        [DataMember]
        public string ActionType { get; set; } 
        #endregion

        #region ResponseChannelId
        /// <summary>
        /// Gets or sets the channelId of the response message.
        /// </summary>
        [DataMember]
        public string ResponseChannelId { get; set; }
        #endregion
        #region ResponseChannelPriority
        /// <summary>
        /// Gets or sets the priority for the ResponseChannelId.
        /// </summary>
        [DataMember]
        public int ResponseChannelPriority { get; set; }
        #endregion
        #region ResponseMessageType
        /// <summary>
        /// Gets or sets the Response message type.
        /// </summary>
        [DataMember]
        public string ResponseMessageType { get; set; }
        #endregion
        #region ResponseActionType
        /// <summary>
        /// Gets or sets the response action type.
        /// </summary>
        [DataMember]
        public string ResponseActionType { get; set; }
        #endregion

        #region Blob
        /// <summary>
        /// Gets or sets the BLOB.
        /// </summary>
        [DataMember]
        public byte[] Blob { get; set; } 
        #endregion

        #region IsNoop
        /// <summary>
        /// Gets or sets a value indicating whether this is a trace message with no-operation
        /// </summary>
        [DataMember]
        public bool IsNoop { get; set; }
        #endregion
        #region IsReplay
        /// <summary>
        /// Gets or sets a value indicating whether this instance is replay.
        /// </summary>
        [DataMember]
        public bool IsReplay { get; set; } 
        #endregion

        #region Status
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [DataMember]
        public string Status { get; set; } 
        #endregion
        #region StatusDescription
        /// <summary>
        /// Gets or sets the status description.
        /// </summary>
        [DataMember]
        public string StatusDescription { get; set; }
        #endregion

        #region FabricDeliveryCount
        /// <summary>
        /// Gets or sets the number of times the message has been sent from the fabric.
        /// </summary>
        public int FabricDeliveryCount { get; set; }
        #endregion        

        #region ToString()
        /// <summary>
        /// This override is used to log the message.
        /// </summary>
        /// <returns>Returns a string that contains the data.</returns>
        public override string ToString()
        {
            return string.Format("Type={0}({1}|{2}) [{3}]", ChannelId, MessageType, ActionType, OriginatorServiceId);
        } 
        #endregion
        #region ToServiceMessageHeader()
        /// <summary>
        /// This method converts the message metadata in to a header.
        /// </summary>
        /// <returns></returns>
        public virtual ServiceMessageHeader ToServiceMessageHeader()
        {
            return new ServiceMessageHeader(ChannelId, MessageType, ActionType);
        } 
        #endregion
        #region ToKey()
        /// <summary>
        /// This method converts the message metadata in to a header.
        /// </summary>
        /// <returns></returns>
        public virtual string ToKey()
        {
            return ServiceMessageHeader.ToKey(ChannelId, MessageType, ActionType);
        }
        #endregion
        #region ToResponseKey()
        /// <summary>
        /// This method converts the message response metadata in to a header.
        /// </summary>
        /// <returns></returns>
        public virtual string ToResponseKey()
        {
            return ServiceMessageHeader.ToKey(ResponseChannelId, ResponseMessageType, ResponseActionType);
        }
        #endregion
    }
}
