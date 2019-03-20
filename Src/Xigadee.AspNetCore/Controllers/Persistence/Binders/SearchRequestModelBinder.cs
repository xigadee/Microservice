using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This binder is used to parse search requests.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ModelBinding.IModelBinder" />
    public class SearchRequestModelBinder : IModelBinder
    {
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchRequestModelBinder"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public SearchRequestModelBinder(ILogger<SearchRequestModelBinder> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Binds the incoming request to a EntityIdModel.
        /// </summary>
        /// <param name="bindingContext">The incoming context.</param>
        /// <returns>This is async.</returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            try
            {
                var q = bindingContext.ActionContext.HttpContext.Request.Query;

                var result = new SearchRequestModel(q.ToString());


                //if (q.ContainsKey(cnSearch))
                //{
                //    result.IsSearch = true;
                //    result.SearchType = q[cnSearch].FirstOrDefault()?.Replace("*", string.Empty);

                //    if (q.ContainsKey(cnOrderBy))
                //        result.OrderBy = q[cnOrderBy].FirstOrDefault();

                //    if (q.ContainsKey(cnSkip))
                //        result.Skip = TryParse(q[cnSkip]);

                //    if (q.ContainsKey(cnTop))
                //        result.Top = TryParse(q[cnTop]);

                //    result.Parameters = q
                //        .Where((t) => !Fields.Contains(t.Key.ToLowerInvariant()))
                //        .Select((t) => new KeyValuePair<string, string>(t.Key, t.Value.FirstOrDefault()))
                //        .ToList();



                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"{nameof(EntityRequestModelBinder)} unhandled exception.");
                bindingContext.Result = ModelBindingResult.Failed();
            }

            return Task.CompletedTask;
        }
    }
}
