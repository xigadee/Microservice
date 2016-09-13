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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee
{
    /// <summary>
    /// This is a very simple command that 
    /// </summary>
    /// <typeparam name="I"></typeparam>
    public class EventTestCommand<I>: CommandBase
        where I : IMessageContract
    {
        /// <summary>
        /// This is the event that the command can be attached to.
        /// </summary>
        public event EventHandler<Tuple<TransmissionPayload, List<TransmissionPayload>>> OnExecute;

        public EventTestCommand(CommandPolicy policy = null) : base(policy)
        {

        }

        protected override void CommandsRegister()
        {
            base.CommandsRegister();
            CommandRegister<I>(ExecuteRequest);
        }

        /// <summary>
        /// This method is called when an entity of the cache type is updated or deleted.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="rs">The responses.</param>
        /// <returns></returns>
        protected virtual async Task ExecuteRequest(TransmissionPayload rq, List<TransmissionPayload> rs)
        {
            if (OnExecute != null)
                OnExecute(this, new Tuple<TransmissionPayload, List<TransmissionPayload>>(rq, rs));
        }
    }

}
