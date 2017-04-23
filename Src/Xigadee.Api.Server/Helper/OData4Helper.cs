#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
