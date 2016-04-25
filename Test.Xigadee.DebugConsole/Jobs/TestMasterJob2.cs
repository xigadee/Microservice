using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using System.Collections.Generic;
using Xigadee;

namespace Test.Xigadee
{
    public class TestMasterJob2: CommandBase
    {
        public TestMasterJob2(string channel) : base(CommandPolicy.ToMasterJob(channel))
        {

        }

        protected override void StartInternal()
        {
            base.StartInternal();

            //var schedule = new Schedule(
            MasterJobRegister(TimeSpan.FromMinutes(5),
                initialWait: TimeSpan.FromSeconds(10),
                action: CallMeAsWell);
        }

        private async Task CallMeAsWell(Schedule schedule)
        {
            //try
            //{
            //    //throw new Exception("Don't care");
            //    var serv = SharedServices.GetService<IRepositoryAsync<Guid, MondayMorningBlues>>();
            //    var result = await serv.Read(new Guid("414f06b5-7c16-403a-acc5-40d2b18f08a1"));
            //}
            //catch (Exception ex)
            //{
            //    //throw;
            //}
        }

    }

}
