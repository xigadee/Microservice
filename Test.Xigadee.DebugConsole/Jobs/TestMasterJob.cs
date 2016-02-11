using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using System.Collections.Generic;
using Xigadee;

namespace Test.Xigadee
{
    public class DoNothingJob: JobBase<JobStatistics>
    {
        public DoNothingJob() : base(JobConfiguration.ToJob(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1),null))
        {
        }
    }

    public class DelayedProcessingJob : JobBase<JobStatistics>
    {
        private Logger mLogger = LogManager.GetCurrentClassLogger();
        private int mCurrentExecutionId;

        public DelayedProcessingJob() : base(JobConfiguration.ToJob(TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(1), null, isLongRunningJob: true))
        {
        }

        protected override void TimerPollSchedulesRegister(JobConfiguration config)
        {
            var job = new Schedule(ExecuteJob, string.Format("ExecuteJob: {0}", GetType().Name));

            job.Frequency = config.Interval;
            job.InitialWait = config.InitialWait;
            job.InitialTime = config.InitialTime;
            job.IsLongRunning = false;

            mSchedules.Add(job);
        }

        protected async Task ExecuteJob(Schedule state, CancellationToken token)
        {
            int currentJob = mCurrentExecutionId++;
            mLogger.Log(LogLevel.Info, "DelayedProcessingJob - Executing schedule " + currentJob);
            state.State = currentJob;
            
            Dispatcher(this, new TransmissionPayload("interserv", "do", "something", options:ProcessOptions.RouteExternal));

            //await Task.Delay(TimeSpan.FromSeconds(40), token);
            mLogger.Log(LogLevel.Info, "DelayedProcessingJob - Finished executing schedule " + currentJob);
        }
    }
    
    public class TestMasterJob : JobBase<JobStatistics>, IRequireSharedServices
    {
        public TestMasterJob(string channel):base(JobConfiguration.ToMasterJob(channel))
        {

        }

        public ISharedService SharedServices
        {
            get;set;
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

    public class TestMasterJob2: JobBase<JobStatistics>, IRequireSharedServices
    {
        public TestMasterJob2(string channel) : base(JobConfiguration.ToMasterJob(channel))
        {

        }

        public ISharedService SharedServices
        {
            get; set;
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
