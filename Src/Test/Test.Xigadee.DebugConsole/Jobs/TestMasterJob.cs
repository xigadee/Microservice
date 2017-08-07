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
    public class TestMasterJob: CommandBase
    {
        public TestMasterJob(string channel) : base()//JobConfiguration.ToMasterJob(channel)
        {

        }

        protected override CommandPolicy PolicyCreateOrValidate(CommandPolicy incomingPolicy)
        {
            var pol = base.PolicyCreateOrValidate(incomingPolicy);

            pol.MasterJobEnabled = true;
            pol.MasterJobNegotiationChannelIdOutgoing = ChannelId;

            return pol;
        }



        protected override void MasterJobCommandsRegister()
        {
            CommandRegister("mychannel", "do", "something", DoSomething);
        }

        private async Task DoSomething(TransmissionPayload rq, List<TransmissionPayload> rs)
        {
            Collector?.LogMessage("all done");
        }

        protected override void MasterJobCommandsUnregister()
        {
            CommandUnregister("mychannel", "do", "something");

        }

        [MasterJobSchedule("00:10:00", "00:00:01")]
        private async Task CallMe(Schedule schedule)
        {
            try
            {
                var id = Guid.NewGuid();
                //throw new Exception("Don't care");
                var serv = SharedServices.GetService<IRepositoryAsync<Guid, MondayMorningBlues>>();
                var result2 = await serv.Create(new MondayMorningBlues() { Id = id });
                var result = await serv.Read(id);
            }
            catch (Exception ex)
            {
                //throw;
            }
        }
 
    }

}
