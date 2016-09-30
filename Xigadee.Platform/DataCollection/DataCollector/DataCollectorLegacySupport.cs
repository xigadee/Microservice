using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class DataCollectorLegacySupport<E,I>: DataCollectorHolder
        where E: EventBase
    {
        I mLegacy;

        public DataCollectorLegacySupport(DataCollectionSupport support, I legacy, Action<I,E> action):base()
        {
            mLegacy = legacy;
            SupportAdd(support, (e) => action(mLegacy, (E)e));
        }

    }
}
