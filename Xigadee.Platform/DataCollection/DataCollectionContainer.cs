using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class DataCollectionContainer: ServiceContainerBase<DataCollectionStatistics, DataCollectionPolicy>
    {
        protected HashSet<ILogger> mLoggers;
        protected HashSet<IEventSource> mEventSource;
        protected HashSet<IBoundaryLogger> mBoundaryLoggers;
        protected HashSet<ITelemetry> mTelemetry;

        public DataCollectionContainer(DataCollectionPolicy policy):base(policy)
        {

        }

        protected override void StartInternal()
        {
        }

        protected override void StopInternal()
        {
        }
    }
}
