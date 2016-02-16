#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    //Autotune
    public partial class MicroserviceBase
    {
        #region Declarations
        private CpuStats mCpuStats = new CpuStats();

        private DateTime? mAutotuneLastProcessorTime = null;

        private int mAutotuneTasksMaxConcurrent = 0;
        private int mAutotuneTasksMinConcurrent = 0;
        private int mAutotuneOverloadTasksConcurrent = 0;

        private long mAutotuneProcessorCurrentMissCount = 0;
        #endregion

        #region Autotune()
        /// <summary>
        /// This method is used to reduce or increase the processes currently active based on the target CPU percentage.
        /// </summary>
        protected virtual async Task Autotune()
        {
            try
            {
                float? current = await mCpuStats.SystemProcessorUsagePercentage(System.Diagnostics.Process.GetCurrentProcess().ProcessName);

                if (!current.HasValue && mAutotuneProcessorCurrentMissCount<5)
                {
                    Interlocked.Increment(ref mAutotuneProcessorCurrentMissCount);
                    return;
                }

                if (!ConfigurationOptions.SupportAutotune)
                    return;

                mAutotuneProcessorCurrentMissCount = 0;
                float processpercentage = current.HasValue? (float)current.Value:0.01F;

                //Do we need to scale down
                if ((processpercentage > ConfigurationOptions.ProcessorTargetLevelPercentage) 
                    || (mAutotuneTasksMaxConcurrent > ConfigurationOptions.ConcurrentRequestsMin))
                {
                    Interlocked.Decrement(ref mAutotuneTasksMaxConcurrent);
                    if (mAutotuneTasksMaxConcurrent < 0)
                        mAutotuneTasksMaxConcurrent = 0;
                }
                //Do we need to scale up
                if ((processpercentage <= ConfigurationOptions.ProcessorTargetLevelPercentage) 
                    && (mAutotuneTasksMaxConcurrent < ConfigurationOptions.ConcurrentRequestsMax))
                {
                    Interlocked.Increment(ref mAutotuneTasksMaxConcurrent);
                    if (mAutotuneTasksMaxConcurrent > ConfigurationOptions.ConcurrentRequestsMax)
                        mAutotuneTasksMaxConcurrent = ConfigurationOptions.ConcurrentRequestsMax;
                }

                int AutotuneOverloadTasksCurrent = mAutotuneTasksMaxConcurrent / 10;

                if (AutotuneOverloadTasksCurrent > ConfigurationOptions.OverloadProcessLimitMax)
                {
                    mAutotuneOverloadTasksConcurrent = ConfigurationOptions.OverloadProcessLimitMax;
                }
                else if (AutotuneOverloadTasksCurrent < ConfigurationOptions.OverloadProcessLimitMin)
                {
                    mAutotuneOverloadTasksConcurrent = ConfigurationOptions.OverloadProcessLimitMin;
                }
                else
                {
                    mAutotuneOverloadTasksConcurrent = AutotuneOverloadTasksCurrent;
                }
            }
            catch (Exception ex)
            {
                //Autotune should not throw an exceptions
                if (mLogger != null)
                    mLogger.LogException("Autotune threw an exception.", ex);
            }
        } 
        #endregion
    }
}
