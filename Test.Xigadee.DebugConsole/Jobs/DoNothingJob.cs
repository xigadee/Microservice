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
}
