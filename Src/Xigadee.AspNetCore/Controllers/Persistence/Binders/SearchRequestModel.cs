using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    [ModelBinder(typeof(SearchRequestModelBinder))]
    public class SearchRequestModel:SearchRequest
    {
        public SearchRequestModel(IQueryCollection query):base()
        {
            query.ForEach(a => Assign(a.Key, a.Value.ToString()));
        }

        public bool IsSearch => IsSet;

        public bool IsSearchEntity => IsSet && string.IsNullOrEmpty(Select);
    }
}
