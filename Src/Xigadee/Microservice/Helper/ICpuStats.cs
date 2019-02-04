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
    /// This interface is implemented by the CpuStats class and is used to share statistics information.
    /// </summary>
    public interface ICpuStats
    {
        /// <summary>
        /// Gets the last calculation time.
        /// </summary>
        DateTime? CalculationTimeLast { get; }
        /// <summary>
        /// Gets the calculation miss count.
        /// </summary>
        long CalculationMissCount { get; }
        /// <summary>
        /// Gets the calculation miss exception.
        /// </summary>
        string CalculationMissException { get; }
        /// <summary>
        /// Gets the service percentage.
        /// </summary>
        float? ServicePercentage { get; }

        /// <summary>
        /// This is the unique client identifier.
        /// </summary>
        int ProcessorCount { get; }
        /// <summary>
        /// This string identifies a 64 or 32 processor.
        /// </summary>
        string ProcessorType { get; }
        /// <summary>
        /// Calculates the system processor usage percentage.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Async float value.</returns>
        Task<float?> SystemProcessorUsagePercentage(string name);
    }
}
