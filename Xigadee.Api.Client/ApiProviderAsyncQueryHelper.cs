using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ApiProviderAsyncQueryHelper
    {
        public static IQueryable<E> Query<K, E>(this ApiProviderAsyncV2<K, E> provider) where K: IEquatable<K>
        {
            throw new NotImplementedException();
        }
    }
}
