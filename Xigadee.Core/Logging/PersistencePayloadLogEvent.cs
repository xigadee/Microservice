#region using

using System;
using System.Collections.Generic;

#endregion
namespace Xigadee
{
    public class PersistencePayloadLogEvent : PayloadLogEvent
    {
        public PersistencePayloadLogEvent(TransmissionPayload payload, RepositoryHolder request, RepositoryHolder response, LoggingLevel? level = null
            , Exception ex = null, DispatcherLoggerDirection? direction = null, TimeSpan? processingTime = null)
            :base(payload, level, ex, direction, processingTime)
        {
            if (request != null)
            {
                TraceId = request.TraceId;
                if (request.Settings != null)
                {
                    BatchId = request.Settings.BatchId;
                    Prefer = request.Settings.Prefer;
                    Headers = request.Settings.Headers;
                }
            }

            if (response != null)
            {
                ResponseCode = response.ResponseCode;
                ResponseMessage = response.ResponseMessage;
            }
        }

        public string BatchId { get; set; }

        public string TraceId { get; set; }

        public Dictionary<string, string> Prefer { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public virtual int ResponseCode { get; set; }

        public virtual string ResponseMessage { get; set; }

        public override string Message
        {
            get
            {
                return string.Format("{0} TraceId={1} BatchId={2} ReponseCode={3} ReponseMessage={4}", base.Message, TraceId, BatchId, ResponseCode, ResponseMessage);
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
                var additionalData = base.AdditionalData;
                additionalData["TraceId"] = TraceId;
                additionalData["BatchId"] = BatchId;
                additionalData["ResponseCode"] = ResponseCode.ToString();
                additionalData["ResponseMessage"] = ResponseMessage;
                return additionalData;
            }
        }
    }
}
