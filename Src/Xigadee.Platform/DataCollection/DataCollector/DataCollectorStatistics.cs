using System;
using System.Collections.Generic;
using System.Linq;

namespace Xigadee
{
    /// <summary>
    /// This class holds the logging statistics for the DataCollector.
    /// </summary>
    public class DataCollectorStatistics: StatusBase
    {
        #region Declarations
        Dictionary<DataCollectionSupport, StatsContainer> mStats;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public DataCollectorStatistics():base()
        {
            //Create the stats container.
            mStats = Enum.GetValues(typeof(DataCollectionSupport))
                .Cast<DataCollectionSupport>()
                .ToDictionary((k) => k, (k) => new StatsContainer());
        }
        #endregion

        #region Name
        /// <summary>
        /// This is the statistics name.
        /// </summary>
        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
            }
        }
        #endregion

        #region ErrorIncrement(DataCollectionSupport support)
        /// <summary>
        /// This method is used to increment the active and total record count.
        /// </summary>
        /// <param name="support">The specific collection type.</param>
        public virtual void ErrorIncrement(DataCollectionSupport support)
        {
            mStats[support].ErrorIncrement();
        }
        #endregion

        #region ActiveIncrement(DataCollectionSupport support)
        /// <summary>
        /// This method is used to increment the active and total record count.
        /// </summary>
        /// <param name="support">The specific collection type.</param>
        public virtual int ActiveIncrement(DataCollectionSupport support)
        {
            return mStats[support].ActiveIncrement();
        }
        #endregion
        #region ActiveDecrement(DataCollectionSupport support, int start)
        /// <summary>
        /// This method is used to decrement the active count and submits the processing time.
        /// </summary>
        /// <param name="support">The specific collection type.</param>
        /// <param name="start">The processing time in milliseconds.</param>
        public virtual int ActiveDecrement(DataCollectionSupport support, int start)
        {
            return mStats[support].ActiveDecrement(start);
        }
        #endregion
        #region ActiveDecrement(DataCollectionSupport support, TimeSpan extent)
        /// <summary>
        /// This method is used to decrement the active count and submits the processing time.
        /// </summary>
        /// <param name="support">The specific collection type.</param>
        /// <param name="extent">The processing time in milliseconds.</param>
        public virtual int ActiveDecrement(DataCollectionSupport support, TimeSpan extent)
        {
            return mStats[support].ActiveDecrement(extent);
        }
        #endregion
    }
}
