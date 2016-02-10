#region using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    public interface ICpuStats
    {
        DateTime? CalculationTime { get; }
        long CalculationMissCount { get; }
        string CalculationMissException { get; }
        float? ServicePercentage { get; }

        /// <summary>
        /// This is the unique client identifier.
        /// </summary>
        int ProcessorCount { get; }
        /// <summary>
        /// This string identifies a 64 or 32 processor.
        /// </summary>
        string ProcessorType { get; }
    }

    /// <summary>
    /// This class contains information about the underlying processor, including the current
    /// processor usage.
    /// </summary>
    internal class CpuStats: ICpuStats
    {

        PerformanceCounter mCpuTotal = null;
        PerformanceCounter mCpuProcess = null;
        private int mMissCount;

        public CpuStats()
        {
            ProcessorCount = Environment.ProcessorCount;
            ProcessorType = Environment.Is64BitProcess ? "64 Bit" : "32 Bit";
        }


        public float? ServicePercentage { get; private set; }
        public DateTime? CalculationTime { get; private set; }
        public long CalculationMissCount { get { return mMissCount; } }
        public string CalculationMissException { get; private set; }

        public int ProcessorCount { get; private set; }

        public string ProcessorType { get; private set; }


        #region SystemProcessorUsagePercentage(string name)
        /// <summary>
        /// This method gets the current processor usage percentage for the Microservice.
        /// </summary>
        /// <returns></returns>
        public async Task<float?> SystemProcessorUsagePercentage(string name)
        {
            try
            {
                if (name == null)
                    name = Process.GetCurrentProcess().ProcessName;

                if (mCpuProcess == null)
                    mCpuProcess = new PerformanceCounter("Process", "% Processor Time", name);
                if (mCpuTotal == null)
                    mCpuTotal = new PerformanceCounter("Process", "% Processor Time", "_Total");

                mCpuTotal.NextValue();
                mCpuProcess.NextValue();
                await Task.Delay(1000);

                float t = mCpuTotal.NextValue();
                float p = mCpuProcess.NextValue();

                ServicePercentage = (p / t) * 100;
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref mMissCount);
                CalculationMissException = ex.Message;
                ServicePercentage = null;
            }

            CalculationTime = DateTime.UtcNow;
            return ServicePercentage;
        }
        #endregion

    }
}
