using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.OData.Query;

namespace Xigadee
{
    /// <summary>
    /// This extension class provides specific OData support.
    /// </summary>
    public static class OData4Helper
    {
        /// <summary>
        /// This extension method is used to load the OData search parameters in to the generic search request object.
        /// </summary>
        /// <typeparam name="E">The Entity type.</typeparam>
        /// <param name="rq">The search object</param>
        /// <param name="incoming">The OData request.</param>
        public static void ODataPopulate<E>(this SearchRequest rq, ODataQueryOptions<E> incoming)
        {           
            //Not very elegant, but we don't want to make a dependencies on OData with SearchRequest.
            rq.Apply = incoming.RawValues.Apply;
            rq.Count = incoming.RawValues.Count;
            rq.DeltaToken = incoming.RawValues.DeltaToken;
            rq.Expand = incoming.RawValues.Expand;
            rq.Filter = incoming.RawValues.Filter;
            rq.Format = incoming.RawValues.Format;
            rq.OrderBy = incoming.RawValues.OrderBy;
            rq.Select = incoming.RawValues.Select;
            rq.Skip = incoming.RawValues.Skip;
            rq.SkipToken = incoming.RawValues.SkipToken;
            rq.Top = incoming.RawValues.Top;
        }

    }
}
