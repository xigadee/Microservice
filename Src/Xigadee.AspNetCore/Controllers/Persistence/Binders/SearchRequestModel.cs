using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    [ModelBinder(typeof(SearchRequestModelBinder))]
    public class SearchRequestModel:SearchRequest
    {
        public SearchRequestModel(string query):base(query)
        {

        }
    }
}
