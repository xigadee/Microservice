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
            return await ProcessOutgoing(channelId, messageType,  actionType, rq, rqSettings, routingOptions, processResponse);
        } 
        #endregion
    }
}
