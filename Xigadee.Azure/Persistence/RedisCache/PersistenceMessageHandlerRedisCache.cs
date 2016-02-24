using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class PersistenceMessageHandlerRedisCache<K, E>: PersistenceManagerHandlerJsonBase<K, E>
        where K : IEquatable<K>
    {
        protected override void ProcessOutputKey(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, IResponseHolder holderResponse)
        {
            throw new NotImplementedException();
        }
    }
}
