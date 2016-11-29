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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;
namespace Xigadee
{
    /// <summary>
    /// This class can be used to handle requests to remote commands
    /// </summary>
    public class CommandInitiator: CommandBase<CommandInitiatorStatistics, CommandInitiatorPolicy>
    {
        #region Declarations
        /// <summary>
        /// This is the default timespan that a message will wait if not set.
        /// </summary>
        protected readonly TimeSpan? mDefaultRequestTimespan;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor which sets the cache manager.
        /// </summary>
        public CommandInitiator(TimeSpan? defaultRequestTimespan = null)
        {
            mDefaultRequestTimespan = defaultRequestTimespan;
        }
        #endregion

        #region Process<I, RQ, RS>(RQ rq, RequestSettings settings = null, ProcessOptions? routing = null)
        /// <summary>
        /// This method is used to send requests to the remote command.
        /// </summary>
        /// <typeparam name="I">The contract interface.</typeparam>
        /// <typeparam name="RQ">The request type.</typeparam>
        /// <typeparam name="RS">The response type.</typeparam>
        /// <param name="rq">The request object.</param>
        /// <param name="routing"></param>
        /// <param name="settings"></param>
        /// <returns>Returns a response object of the specified type in a response metadata wrapper.</returns>
        public virtual async Task<ResponseWrapper<RS>> Process<I, RQ, RS>(RQ rq
            , RequestSettings settings = null
            , ProcessOptions? routing = null)
            where I : IMessageContract
        {
            string channelId, messageType, actionType;

            if (!ServiceMessageHelper.ExtractContractInfo<I>(out channelId, out messageType, out actionType))
                throw new InvalidOperationException("Unable to locate message contract attributes for " + typeof(I));

            return await Process<RQ, RS>(channelId, messageType, actionType, rq, settings, routing);
        }
        #endregion

        #region Process<RQ, RS> ...
        /// <summary>
        /// This method is used to send requests to the remote command.
        /// </summary>
        /// <typeparam name="RQ">The request type.</typeparam>
        /// <typeparam name="RS">The response type.</typeparam>
        /// <param name="channelId">The header routing information.</param>
        /// <param name="messageType">The header routing information.</param>
        /// <param name="actionType">The header routing information.</param>
        /// <param name="rq">The request object.</param>
        /// <param name="rqSettings"></param>
        /// <param name="routingOptions">The routing options by default this will try internal and then external.</param>
        /// <param name="processResponse"></param>
        /// <returns></returns>
        public virtual async Task<ResponseWrapper<RS>> Process<RQ, RS>(
              string channelId, string messageType, string actionType
            , RQ rq
            , RequestSettings rqSettings = null
            , ProcessOptions? routingOptions = null
            , Func<TaskStatus, TransmissionPayload, bool, ResponseWrapper<RS>> processResponse = null
            )
        {
            TransmissionPayload payload = null;
            try
            {
                StatisticsInternal.ActiveIncrement();

                payload = TransmissionPayload.Create();

                // Set the process correlation key to the correlation id if passed through the rq settings
                if (!string.IsNullOrEmpty(rqSettings?.CorrelationId))
                    payload.Message.ProcessCorrelationKey = rqSettings.CorrelationId;

                bool processAsync = rqSettings?.ProcessAsync ?? false;

                payload.Options = routingOptions ?? ProcessOptions.RouteExternal | ProcessOptions.RouteInternal;

                //Set the destination message
                payload.Message.ChannelId = channelId ?? ChannelId;
                payload.Message.MessageType = messageType;
                payload.Message.ActionType = actionType;
                payload.Message.ChannelPriority = processAsync ? 0 : 1;

                //Set the response path
                payload.Message.ResponseChannelId = ResponseId.Header.ChannelId;
                payload.Message.ResponseMessageType = ResponseId.Header.MessageType;
                payload.Message.ResponseActionType = ResponseId.Header.ActionType;
                payload.Message.ResponseChannelPriority = payload.Message.ChannelPriority;

                //Set the payload
                payload.Message.Blob = PayloadSerializer.PayloadSerialize(rq);

                //Set the processing time
                payload.MaxProcessingTime = rqSettings?.WaitTime ?? mDefaultRequestTimespan;

                //Transmit
                return await TransmitAsync(payload, processResponse ?? ProcessResponse<RS>, processAsync);
            }
            catch (Exception ex)
            {
                string key = payload?.Id.ToString() ?? string.Empty;
                Logger.LogException(string.Format("Error transmitting {0}-{1} internally", actionType, key), ex);
                throw;
            }
        } 
        #endregion

        #region ProcessResponse<KT, ET>(TaskStatus status, TransmissionPayload prs, bool async)
        /// <summary>
        /// This method is used to process the returning message response.
        /// </summary>
        /// <typeparam name="RS"></typeparam>
        /// <param name="rType"></param>
        /// <param name="payload"></param>
        /// <param name="processAsync"></param>
        /// <returns></returns>
        protected virtual ResponseWrapper<RS> ProcessResponse<RS>(TaskStatus rType, TransmissionPayload payload, bool processAsync)
        {
            StatisticsInternal.ActiveDecrement(payload != null ? payload.Extent : TimeSpan.Zero);
            if (processAsync)
                return new ResponseWrapper<RS>(responseCode: 202, responseMessage: "Accepted");

            payload.CompleteSet();

            try
            {
                switch (rType)
                {
                    case TaskStatus.RanToCompletion:
                        try
                        {                        
                            //payload.Message.
                            var response = new ResponseWrapper<RS>(payload);

                            if (payload.MessageObject != null)
                                response.Response = (RS)payload.MessageObject;
                            else if (payload.Message.Blob != null)
                                response.Response = PayloadSerializer.PayloadDeserialize<RS>(payload);

                            return response;
                        }
                        catch (Exception ex)
                        {
                            StatisticsInternal.ErrorIncrement();
                            return new ResponseWrapper<RS>(500, ex.Message);
                        }
                    case TaskStatus.Canceled:
                        StatisticsInternal.ErrorIncrement();
                        return new ResponseWrapper<RS>(408, "Time out");
                    case TaskStatus.Faulted:
                        StatisticsInternal.ErrorIncrement();
                        return new ResponseWrapper<RS>((int)PersistenceResponse.GatewayTimeout504, "Response timeout.");
                    default:
                        StatisticsInternal.ErrorIncrement();
                        return new ResponseWrapper<RS>(500, rType.ToString());

                }
            }
            catch (Exception ex)
            {
                Logger.LogException("Error processing response for task status " + rType, ex);
                throw;
            }
        }
        #endregion

    }
}
