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
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    public static class PayloadLogEventHelper
    {
        public static void LogPayload(this ILogger logger, TransmissionPayload payload, LoggingLevel? level = null
            , Exception ex = null, DispatcherLoggerDirection? direction = null, TimeSpan? timespan = null)
        {
            logger.Log(new PayloadLogEvent(payload, level, ex, direction, timespan));
        }
    }

    public class PayloadLogEvent:LogEvent
    {
        public PayloadLogEvent(TransmissionPayload payload, LoggingLevel? level = null
            , Exception ex = null, DispatcherLoggerDirection? direction = null, TimeSpan? processingTime = null)
        {
            Ex = ex;

            IsDeadLetterMessage = payload.IsDeadLetterMessage;

            MessageHeader = payload.Message.ToServiceMessageHeader().ToKey();

            CorrelationUTC = payload.Message.CorrelationUTC;

            MessageId = payload.Message.CorrelationKey;

            OriginatorUTC = payload.Message.OriginatorUTC;

            CorrelationId = payload.Message.OriginatorKey ?? MessageId;

            base.Level = level ?? ((ex != null) ? LoggingLevel.Error : LoggingLevel.Trace);

            Direction = direction;

            ProcessingTime = processingTime;

            TimeStamp = DateTime.UtcNow;
        }

        public DispatcherLoggerDirection? Direction { get; set; }

        public string CorrelationId { get; set; }

        public DateTime? OriginatorUTC { get; set; }

        public string MessageId { get; set; }

        public DateTime? CorrelationUTC { get; set; }

        public bool IsDeadLetterMessage { get; set; }

        public string MessageHeader { get; set; }

        public TimeSpan? ProcessingTime { get; set; }

        public string ExceptionType { get { return Ex == null ? (string)null : Ex.GetType().Name; } }

        public DateTime TimeStamp { get; private set; }

        public override string Message
        {
            get
            {
                return string.Format("{0} Payload {1}/{4} CorrelationId={2} MessageId={3}", base.Message, Direction, CorrelationId, MessageId, Level);
            }
            set
            {
                base.Message = value;
            }
        }

        public override Dictionary<string, string> AdditionalData            
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"CorrelationId", CorrelationId},
                    {"MessageId", MessageId},
                    {"MessageHeader", MessageHeader},
                    {"IsDeadLetterMessage", IsDeadLetterMessage.ToString()}
                };
            }
        }
    }
}
