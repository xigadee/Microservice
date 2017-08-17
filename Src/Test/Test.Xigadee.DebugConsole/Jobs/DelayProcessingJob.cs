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
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xigadee;

namespace Test.Xigadee
{
    public class DelayedProcessingJob : CommandBase
    {
        //private Logger mLogger = LogManager.GetCurrentClassLogger();
        private int mCurrentExecutionId;

        public DelayedProcessingJob() : base(CommandPolicy.ToJob(TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(1), null, isLongRunningJob: false))
        {
        }

        protected override void JobSchedulesManualRegister()
        {           
            JobScheduleRegister(ExecuteJob, name: $"DelayedProcessingJob: {GetType().Name}");
        }

        protected async Task ExecuteJob(Schedule state, CancellationToken token)
        {
            int currentJob = mCurrentExecutionId++;
            //mCollector?.Log(LogLevel.Info, "DelayedProcessingJob - Executing schedule " + currentJob);
            state.Context = currentJob;
            var payload = new TransmissionPayload("interserv", "do", "something", options:ProcessOptions.RouteExternal);
            TaskManager(this, null, payload);

            //await Task.Delay(TimeSpan.FromSeconds(40), token);
            //mCollector?.Log(LogLevel.Info, "DelayedProcessingJob - Finished executing schedule " + currentJob);
        }
    }
 
}
