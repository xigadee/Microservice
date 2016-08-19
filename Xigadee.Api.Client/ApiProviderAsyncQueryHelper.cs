using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

    //public class ContactEntityTypeSerializer: ODataEntityTypeSerializer
    //{
    //}

    //internal class MyLinq<K, E>: IOrderedQueryable<E>
    //    where K : IEquatable<K>
    //{

    //    QueryProvider provider;

    //    Expression expression;


    //    public Type ElementType
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public Expression Expression
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public IQueryProvider Provider
    //    {
    //        get
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //    public IEnumerator<E> GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
