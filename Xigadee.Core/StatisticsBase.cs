#region using
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is the base class for components that need to records performance statistics.
    /// </summary>
    /// <typeparam name="S">The base class.</typeparam>
    public abstract class StatisticsBase<S>: IStatisticsBase
        where S : StatusBase, new()
    {
        #region Declarations
        /// <summary>
        /// This is the statistics class that can be referenced throughout the application.
        /// </summary>
        protected S mStatistics;

        private readonly Guid mComponentId;

        private readonly string mName;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constrcutor.
        /// </summary>
        /// <param name="name">This sets the name in the statistics.</param>
        protected StatisticsBase(string name = null)
        {
            mName = name;
            mComponentId = Guid.NewGuid();
            mStatistics = StatisticsCreate();
        } 
        #endregion
        #region Statistics
        /// <summary>
        /// This is the public statistics object
        /// </summary>
        public virtual S Statistics
        {
            get
            {
                StatisticsRecalculate();
                return mStatistics;
            }
        }
        #endregion

        #region StatisticsRecalculate()
        /// <summary>
        /// This method is used to set any calculated fields before the statistics are retrieved.
        /// </summary>
        protected virtual void StatisticsRecalculate()
        {

        }
        #endregion
        #region StatisticsCreate()
        /// <summary>
        /// This method is used to set any calculated fields before the statistics are retrieved.
        /// </summary>
        protected virtual S StatisticsCreate()
        {
            mStatistics = new S();

            mStatistics.Name = mName ?? GetType().Name;
            mStatistics.ComponentId = mComponentId;

            return mStatistics;
        }
        #endregion

        #region StatisticsGet()
        /// <summary>
        /// This method allows for other classes to access the statistics class without a reference to the specific generic class.
        /// </summary>
        /// <returns>Returns the base statistics collection class</returns>
        public StatusBase StatisticsGet()
        {
            return Statistics;
        } 
        #endregion
    }
}
