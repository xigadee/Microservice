using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is responsible for managing external dependencies and resources, particularly the rate limiting and circuit breaker limits.
    /// </summary>
    public class DependencyManager: ServiceBase<DependencyManagerStatistics>, IServiceLogger, IRequireSharedServices
    {
        public DependencyManager()
        {

        }

        /// <summary>
        /// This is the logger reference.
        /// </summary>
        public ILoggerExtended Logger
        {
            get;set;
        }

        /// <summary>
        /// Shared services are used to interact with the dependency manager,
        /// </summary>
        public ISharedService SharedServices
        {
            get;set;
        }

        protected override void StartInternal()
        {
        }

        protected override void StopInternal()
        {
        }
    }
}
