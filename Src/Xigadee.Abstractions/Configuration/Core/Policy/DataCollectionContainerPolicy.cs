using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    #region DataCollectionContainerPolicy
    /// <summary>
    /// This class holds the data collection policy.
    /// </summary>
    public class DataCollectionContainerPolicy : PolicyBase
    {
        /// <summary>
        /// This is the maximum time that an overload process should run.
        /// </summary>
        public int OverloadProcessTimeInMs { get; set; } = 10000; //10s
                                                                  /// <summary>
                                                                  /// This is the maximum number of overload tasks that should be run concurrently.
                                                                  /// </summary>
        public int OverloadMaxTasks { get; set; } = 2;
        /// <summary>
        /// This is the threshold at which point the overload tasks will be triggered.
        /// </summary>
        public int? OverloadThreshold { get; set; } = 1000;
        /// <summary>
        /// This is the number of retry attempts to be made if the write fails.
        /// </summary>
        public int RetryLimit { get; set; } = 0;

        /// <summary>
        /// This is the event source retry limit.
        /// </summary>
        public int EventSourceRetryLimit { get; set; } = 3;
    }
    #endregion

}
