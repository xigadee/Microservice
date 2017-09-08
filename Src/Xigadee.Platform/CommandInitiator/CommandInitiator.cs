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
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class can be used to handle requests to remote commands
    /// </summary>
    public class CommandInitiator : CommandBase<CommandInitiatorStatistics, CommandInitiatorPolicy>, ICommandInitiator
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor which sets the cache manager.
        /// </summary>
        public CommandInitiator(TimeSpan? defaultRequestTimespan = null)
        {
            mPolicy.OutgoingRequestDefaultTimespan = defaultRequestTimespan;
        }
        #endregion

        #region Process<I, RQ, RS>(RQ rq, RequestSettings settings = null, ProcessOptions? routing = null)
        /// <summary>
        /// This method is used to send requests to a remote command and wait for a response.
        /// </summary>
        /// <typeparam name="I">The contract interface.</typeparam>
        /// <typeparam name="RQ">The request object type.</typeparam>
        /// <typeparam name="RS">The response object type.</typeparam>
        /// <param name="rq">The request object.</param>
        /// <param name="settings">The request settings. Use this to specifically set the timeout parameters, amongst other settings, or to pass meta data to the calling party.</param>
        /// <param name="routing">The routing options by default this will try internal and then external endpoints.</param>
        /// <param name="principal">This is the principal that you wish the command to be executed under. 
        /// By default this is taken from the calling thread if not passed.</param>
        /// <returns>Returns the async response wrapper.</returns>
        public virtual async Task<ResponseWrapper<RS>> Process<I, RQ, RS>(
              RQ rq
            , RequestSettings settings = null
            , ProcessOptions? routing = default(ProcessOptions?)
            , IPrincipal principal = null
            )
            where I : IMessageContract
        {
            return await Outgoing.Process<I, RQ, RS>(rq
                , settings
                , routing
                , principal: principal ?? Thread.CurrentPrincipal
                );
        }
        #endregion
        #region Process<RQ, RS> ...
        /// <summary>
        /// This method is used to send requests to a remote command and wait for a response.
        /// </summary>
        /// <typeparam name="RQ">The request object type.</typeparam>
        /// <typeparam name="RS">The response object type.</typeparam>
        /// <param name="channelId"></param>
        /// <param name="messageType"></param>
        /// <param name="actionType"></param>
        /// <param name="rq">The request object.</param>
        /// <param name="settings">The request settings. Use this to specifically set the timeout parameters, amongst other settings, or to pass meta data to the calling party.</param>
        /// <param name="routing">The routing options by default this will try internal and then external endpoints.</param>
        /// <param name="principal">This is the principal that you wish the command to be executed under. 
        /// By default this is taken from the calling thread if not passed.</param>
        /// <returns>Returns the async response wrapper.</returns>
        public virtual async Task<ResponseWrapper<RS>> Process<RQ, RS>(
              string channelId, string messageType, string actionType
            , RQ rq
            , RequestSettings settings = null
            , ProcessOptions? routing = default(ProcessOptions?)
            , IPrincipal principal = null
            )
        {
            return await Outgoing.Process<RQ,RS>(
                  channelId, messageType, actionType
                , rq
                , settings
                , routing
                , principal: principal ?? Thread.CurrentPrincipal);
        }

        /// <summary>
        /// This method is used to send requests to a remote command and wait for a response.
        /// </summary>
        /// <typeparam name="RQ">The request object type.</typeparam>
        /// <typeparam name="RS">The response object type.</typeparam>
        /// <param name="header">The message header object that defines the remote endpoint.</param>
        /// <param name="rq">The request object.</param>
        /// <param name="settings">The request settings. Use this to specifically set the timeout parameters, amongst other settings, or to pass meta data to the calling party.</param>
        /// <param name="routing">The routing options by default this will try internal and then external endpoints.</param>
        /// <param name="principal">This is the principal that you wish the command to be executed under. 
        /// By default this is taken from the calling thread if not passed.</param>
        /// <returns>Returns the async response wrapper.</returns>
        public virtual async Task<ResponseWrapper<RS>> Process<RQ, RS>(
              ServiceMessageHeader header
            , RQ rq
            , RequestSettings settings = null
            , ProcessOptions? routing = default(ProcessOptions?)
            , IPrincipal principal = null
            )
        {
            return await Outgoing.Process<RQ, RS>(
                  header
                , rq
                , settings
                , routing
                , principal: principal ?? Thread.CurrentPrincipal);
        }
        #endregion
    }
}
