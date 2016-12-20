using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This wrapper holds a singular legacy collection component.
    /// </summary>
    /// <typeparam name="E">The eventbase type.</typeparam>
    /// <typeparam name="I">The legacy component type.</typeparam>
    public class DataCollectorLegacySupport<E,I>: DataCollectorBase<DataCollectorStatistics>
        where E: EventBase
    {
        I mLegacy;
        DataCollectionSupport mLegacySupport;
        Action<I, E> mLegacyAction;

        /// <summary>
        /// This class can be used to support legacy collection components.
        /// </summary>
        /// <param name="support">The support type.</param>
        /// <param name="legacy">The legacy interface.</param>
        /// <param name="action">The legacy action.</param>
        public DataCollectorLegacySupport(DataCollectionSupport support, I legacy, Action<I,E> action):base(support)
        {
            if (support == DataCollectionSupport.None)
                throw new ArgumentOutOfRangeException("support", "support must have a value");

            if (legacy == null)
                throw new ArgumentNullException("legacy", "legacy cannot be null");

            if (action == null)
                throw new ArgumentNullException("action", "action cannot be null");

            mLegacy = legacy;
            mLegacySupport = support;
            mLegacyAction = action;
        }

        /// <summary>
        /// This method projects the legacy action in to the collector support methods.
        /// </summary>
        protected override void SupportLoadDefault()
        {
            SupportAdd(mLegacySupport, (e) => mLegacyAction(mLegacy, (E)e));
        }

    }
}
