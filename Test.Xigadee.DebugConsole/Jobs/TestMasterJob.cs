using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using System.Collections.Generic;
using Xigadee;

namespace Test.Xigadee
{
    public class TestMasterJob : CommandBase
    {
        public TestMasterJob(string channel):base()//JobConfiguration.ToMasterJob(channel)
        {

        }

        protected override CommandPolicy PolicyCreateOrValidate(CommandPolicy incomingPolicy)
        {
            var pol = base.PolicyCreateOrValidate(incomingPolicy);

            pol.MasterJobEnabled = true;
            pol.MasterJobNegotiationChannelId = ChannelId;

            return pol;
        }

        protected override void StartInternal()
        {
            base.StartInternal();

            //var schedule = new Schedule(
            MasterJobRegister(TimeSpan.FromSeconds(1), 
                initialWait: TimeSpan.FromSeconds(10),
                action: CallMe);
        }

        protected override void MasterJobCommandsRegister()
        {
            CommandRegister("mychannel", "do", "something", DoSomething);
        }

        private async Task DoSomething(TransmissionPayload rq, List<TransmissionPayload> rs)
        {
            Logger.LogMessage("all done");
        }

        protected override void MasterJobCommandsUnregister()
        {
            CommandUnregister("mychannel", "do", "something");

        }

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
