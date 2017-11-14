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
using System.Threading;

namespace Xigadee
{
    /// <summary>
    /// This is the base fabric message. It closely mirrors the BrokeredMessage of Service Bus to allow for simulations of functionality without the need 
    /// for a work service bus to test.
    /// </summary>
    public class FabricMessage
    {
        /// <summary>
        /// This action is used to signal that the message can be released back to the listener.
        /// </summary>
        private Action<bool, Guid> mSignalRelease = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="FabricMessage"/> class.
        /// </summary>
        public FabricMessage()
        {
            Id = Guid.NewGuid();
            Properties = new Dictionary<string, string>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="FabricMessage"/> class.
        /// </summary>
        /// <param name="blob">The message BLOB.</param>
        public FabricMessage(byte[] blob):this()
        {
            Message = blob;
        }
        /// <summary>
        /// Gets the enqueued time UTC.
        /// </summary>

        public DateTime EnqueuedTimeUtc { get; } = DateTime.UtcNow;
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; }
        /// <summary>
        /// Gets or sets the correlation identifier.
        /// </summary>
        public string CorrelationId { get; set; }
        /// <summary>
        /// Gets the properties dictionary.
        /// </summary>

        public virtual Dictionary<string, string> Properties { get; }
        /// <summary>
        /// Gets or sets the binary message.
        /// </summary>
        public virtual byte[] Message { get; set; }
        /// <summary>
        /// Gets or sets the delivery count.
        /// </summary>
        public virtual int DeliveryCount { get; set; } = 0;

        /// <summary>
        /// Signals the message as succeeded or failed atomically.
        /// If the release action is set, this will be called the first time this method is called.
        /// </summary>
        /// <param name="success">if set to <c>true</c> [success].</param>
        public virtual void Signal(bool success)
        {
            //We only want to do this once.
            var release = Interlocked.Exchange<Action<bool, Guid>>(ref mSignalRelease, null);

            if (release != null)
                try
                {
                    release(success, Id);
                }
                catch (Exception ex)
                {
                }
        }

        /// <summary>
        /// Sets the release function for the message.
        /// </summary>
        /// <param name="release">The release function.</param>
        internal void ReleaseSet(Action<bool, Guid> release)
        {
            mSignalRelease = release;
        }
        /// <summary>
        /// Marks the message as complete and successfully delivered.
        /// </summary>
        public virtual void Complete()
        {
            Signal(true);
        }
        /// <summary>
        /// Abandons this message.
        /// </summary>
        public virtual void Abandon()
        {
            Signal(false);
        }


        #region Pack(TransmissionPayload payload)
        /// <summary>
        /// This method packs the ServiceMessage in to the FabricMessage format
        /// for communication through the manual bus. This is used to mimic similar operations in services such as Azure Service Bus.
        /// </summary>
        /// <param name="payload">The payload object to convert to a FabricMessage.</param>
        /// <returns>Returns a FabricMessage object.</returns>
        public static FabricMessage Pack(TransmissionPayload payload)
        {
            ServiceMessage sMessage = payload.Message;
            FabricMessage bMessage;
            if (sMessage.Blob == null)
                bMessage = new FabricMessage();
            else
                bMessage = new FabricMessage(sMessage.Blob);

            bMessage.Properties.Add("SecuritySignature", sMessage.SecuritySignature);

            bMessage.Properties.Add("OriginatorKey", sMessage.OriginatorKey);
            bMessage.Properties.Add("OriginatorServiceId", sMessage.OriginatorServiceId);
            bMessage.Properties.Add("OriginatorUTC", sMessage.OriginatorUTC.ToString("s", System.Globalization.CultureInfo.InvariantCulture));

            bMessage.Properties.Add("ResponseChannelId", MessagingHelper.ToSafeLower(sMessage.ResponseChannelId));
            bMessage.Properties.Add("ResponseChannelPriority", sMessage.ResponseChannelPriority.ToString());
            bMessage.Properties.Add("ResponseMessageType", MessagingHelper.ToSafeLower(sMessage.ResponseMessageType));
            bMessage.Properties.Add("ResponseActionType", MessagingHelper.ToSafeLower(sMessage.ResponseActionType));

            //FIX: Case sensitive pattern match in ServiceBus.
            bMessage.Properties.Add("ChannelId", MessagingHelper.ToSafeLower(sMessage.ChannelId));
            bMessage.Properties.Add("MessageType", MessagingHelper.ToSafeLower(sMessage.MessageType));
            bMessage.Properties.Add("ActionType", MessagingHelper.ToSafeLower(sMessage.ActionType));

            bMessage.Properties.Add("IsNoop", sMessage.IsNoop ? "1" : "0");
            bMessage.Properties.Add("IsReplay", sMessage.IsReplay ? "1" : "0");

            bMessage.CorrelationId = sMessage.CorrelationServiceId;

            bMessage.Properties.Add("ProcessCorrelationKey", sMessage.ProcessCorrelationKey);

            bMessage.Properties.Add("CorrelationKey", sMessage.CorrelationKey);
            bMessage.Properties.Add("CorrelationServiceId", MessagingHelper.ToSafeLower(sMessage.CorrelationServiceId));
            bMessage.Properties.Add("CorrelationUTC", sMessage.CorrelationUTC.HasValue ? sMessage.CorrelationUTC.Value.ToString("o") : null);

            bMessage.Properties.Add("DispatcherTransitCount", sMessage.DispatcherTransitCount.ToString());

            bMessage.Properties.Add("Status", sMessage.Status);
            bMessage.Properties.Add("StatusDescription", sMessage.StatusDescription);

            return bMessage;
        }
        #endregion
        #region Unpack(FabricMessage bMessage)
        /// <summary>
        /// This method unpacks the FabricMessage message in to a generic ServiceMessage object
        /// which can be processed by the service..
        /// /// </summary>
        /// <param name="bMessage">The FabricMessage to convert.</param>
        /// <returns>Returns a generic ServiceMessage class for processing.</returns>
        public static ServiceMessage Unpack(FabricMessage bMessage)
        {
            var sMessage = new ServiceMessage();

            if (bMessage.Properties.ContainsKey("SecuritySignature"))
                sMessage.SecuritySignature = bMessage.Properties["SecuritySignature"] as string;

            sMessage.EnqueuedTimeUTC = bMessage.EnqueuedTimeUtc;

            sMessage.OriginatorKey = bMessage.Properties["OriginatorKey"] as string;
            sMessage.OriginatorServiceId = bMessage.Properties["OriginatorServiceId"] as string;
            sMessage.OriginatorUTC = DateTime.Parse(bMessage.Properties["OriginatorUTC"]);

            sMessage.ResponseChannelId = bMessage.Properties["ResponseChannelId"] as string;
            if (bMessage.Properties.ContainsKey("ResponseMessageType"))
                sMessage.ResponseMessageType = bMessage.Properties["ResponseMessageType"] as string;
            if (bMessage.Properties.ContainsKey("ResponseActionType"))
                sMessage.ResponseActionType = bMessage.Properties["ResponseActionType"] as string;

            if (bMessage.Properties.ContainsKey("ResponseChannelPriority"))
            {
                string value = bMessage.Properties["ResponseChannelPriority"] as string;
                int responsePriority;
                if (string.IsNullOrEmpty(value) || !int.TryParse(value, out responsePriority))
                    responsePriority = 0;
                sMessage.ResponseChannelPriority = responsePriority;
            }

            sMessage.ChannelId = bMessage.Properties["ChannelId"] as string;
            sMessage.MessageType = bMessage.Properties["MessageType"] as string;
            sMessage.ActionType = bMessage.Properties["ActionType"] as string;

            sMessage.IsNoop = bMessage.Properties["IsNoop"] as string == "1";
            sMessage.IsReplay = bMessage.Properties["IsReplay"] as string == "1";

            if (bMessage.Properties.ContainsKey("ProcessCorrelationKey"))
                sMessage.ProcessCorrelationKey = bMessage.Properties["ProcessCorrelationKey"] as string;

            sMessage.CorrelationKey = bMessage.Properties["CorrelationKey"] as string;
            sMessage.CorrelationServiceId = bMessage.Properties["CorrelationServiceId"] as string;
            DateTime serviceUTC;
            if (bMessage.Properties.ContainsKey("CorrelationUTC") &&
                DateTime.TryParse(bMessage.Properties["CorrelationUTC"] as string, out serviceUTC))
                sMessage.CorrelationUTC = serviceUTC;

            sMessage.DispatcherTransitCount = int.Parse(bMessage.Properties["DispatcherTransitCount"]);

            sMessage.Status = bMessage.Properties["Status"] as string;
            sMessage.StatusDescription = bMessage.Properties["StatusDescription"] as string;

            sMessage.Blob = bMessage.Message;

            sMessage.FabricDeliveryCount = bMessage.DeliveryCount;

            return sMessage;
        }
        #endregion

    }
}
