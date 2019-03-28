using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    public class CombinedRequestModelBinder : IModelBinder
    {
        #region Declarations
        private readonly ILogger _logger;

        const string cnRefType = "reftype";
        const string cnRefValue = "refvalue";

        private static readonly string[] Fields = new[] { cnRefType, cnRefValue };
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityRequestModelBinder"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public CombinedRequestModelBinder(ILogger<EntityRequestModelBinder> logger)
        {
            _logger = logger;
        }
        #endregion

        private int? TryParse(IEnumerable<string> items)
        {
            var item = items.FirstOrDefault();

            int value;
            if (int.TryParse(item, out value))
                return value;

            return null;
        }

        /// <summary>
        /// Binds the incoming request to a EntityIdModel.
        /// </summary>
        /// <param name="bindingContext">The incoming context.</param>
        /// <returns>This is async.</returns>
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var result = new CombinedRequestModel();

            try
            {
                result.EntityRequest = EntityRequestModelBinder.Bind(bindingContext);

                if (result.EntityRequest.IsByKey || result.EntityRequest.IsByReference)
                {
                    result.Type = CombinedRequestModelType.ReadEntity;
                }
                else
                {
                    var q = bindingContext.ActionContext.HttpContext.Request.Query;
                    result.SearchRequest = new SearchRequestModel(q);

                    if (result.SearchRequest.IsSearchEntity)
                        result.Type = CombinedRequestModelType.SearchEntity;
                    else if (result.SearchRequest.IsSearch)
                        result.Type = CombinedRequestModelType.Search;
                }

                if (result.Type.HasValue)
                {
                    bindingContext.Result = ModelBindingResult.Success(result);
                    return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"{nameof(EntityRequestModelBinder)} unhandled exception.");
            }

            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }
    }
}
