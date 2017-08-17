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
    public class TestMasterJob2: CommandBase
    {
        public TestMasterJob2(string channel) : base(CommandPolicy.ToMasterJob(channel))
        {
        }


        [MasterJobSchedule("CallMe")]
        private async Task CallMeAsWell(Schedule schedule)
        {
            try
            {
                //throw new Exception("Don't care");
                var serv = SharedServices.GetService<IRepositoryAsync<Guid, MondayMorningBlues>>();
                var result = await serv.Read(new Guid("414f06b5-7c16-403a-acc5-40d2b18f08a1"));
            }
            catch (Exception ex)
            {
                //throw;
            }
        }

    }

}
