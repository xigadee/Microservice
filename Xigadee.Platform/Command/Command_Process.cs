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
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
#endregion
namespace Xigadee
{
    public abstract partial class CommandBase<S, P, H>
    {
        #region --> ProcessMessage(TransmissionPayload payload, List<TransmissionPayload> responses)
        /// <summary>
        /// This method is called to process an incoming payload.
        /// </summary>
        /// <param name="requestPayload">The message to process.</param>
        /// <param name="responses">The return path for the message.</param>
        public virtual async Task ProcessMessage(TransmissionPayload requestPayload, List<TransmissionPayload> responses)
        {
            int start = StatisticsInternal.ActiveIncrement();

            try
            {
                var header = requestPayload.Message.ToServiceMessageHeader();

                H handler;
                if (!SupportedResolve(header, out handler))
                    throw new CommandNotSupportedException(requestPayload.Id, header, GetType());

                //Call the registered command.
                await handler.Execute(requestPayload, responses);
            }
            catch (Exception)
            {
                StatisticsInternal.ErrorIncrement();
                throw;
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(start);
            }
        }
        #endregion
    }
}
