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

        public DateTime? CalculationTimeLast { get; private set; }

        public long CalculationMissCount { get { return mMissCount; } }

        public string CalculationMissException { get; private set; }
        /// <summary>
        /// This is the number of processors reported on the underlying infrastructure.
        /// </summary>
        public int ProcessorCount { get; private set; }

        public string ProcessorType { get; private set; }

        public string ProcessName { get; private set; }

        #region SystemProcessorUsagePercentage(string name)
        /// <summary>
        /// This method gets the current processor usage percentage for the Microservice.
        /// </summary>
        /// <returns></returns>
        public async Task<float?> SystemProcessorUsagePercentage(string name)
        {
            try
            {
                if (name == null || ProcessName == null)
                    ProcessName = Process.GetCurrentProcess().ProcessName;

                var categories = PerformanceCounterCategory.GetCategories();

                if (mCpuProcess == null)
                    mCpuProcess = new PerformanceCounter("Process", "% Processor Time", ProcessName);
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

            CalculationTimeLast = DateTime.UtcNow;
            return ServicePercentage;
        }
        #endregion

    }
}
