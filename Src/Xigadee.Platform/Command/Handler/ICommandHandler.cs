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
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is used for command handlers to implement, to allow the mapping of incoming requests to their specific commands.
    /// </summary>
    public interface ICommandHandler
    {
        void Initialise(CommandHolder holder);

        MessageFilterWrapper Key { get; }

        Func<TransmissionPayload, List<TransmissionPayload>, Task> Action { get; }

        Task Execute(TransmissionPayload rq, List<TransmissionPayload> rs);

        ICommandHandlerStatistics HandlerStatistics { get; }
    }
}