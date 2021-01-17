using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This class binds the incoming request query to the EntityIdModel.
    /// It support Read, RedByReference and Search in one entity for the GET verb.
    /// </summary>
    public class EntityRequestModelBinder : IModelBinder
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
        public EntityRequestModelBinder(ILogger<EntityRequestModelBinder> logger)
        {
            _logger = logger;
        } 
        #endregion

        private static int? TryParse(IEnumerable<string> items)
        {
            var item = items.FirstOrDefault();

            int value;
            if (int.TryParse(item, out value))
                return value;

            return null;
        }

        /// <summary>
        /// Binds the entity to specified binding context.
        /// </summary>
        /// <param name="bindingContext">The binding context.</param>
        /// <returns>The model.</returns>
        public static EntityRequestModel Bind(ModelBindingContext bindingContext)
        {
            var result = new EntityRequestModel();

            var q = bindingContext.ActionContext.HttpContext.Request.Query;

            result.Id = bindingContext.ValueProvider.GetValue("id1").FirstOrDefault();
            result.IsByKey = !string.IsNullOrEmpty(result.Id);

            result.VersionId = bindingContext.ValueProvider.GetValue("id2").FirstOrDefault();

            if (!result.IsByKey)
            {
                if (q.ContainsKey(cnRefType))
                {
                    result.Reftype = q[cnRefType].FirstOrDefault();
                    result.IsByReference = !string.IsNullOrEmpty(result.Reftype);
                }
                if (q.ContainsKey(cnRefValue))
                    result.Refvalue = q[cnRefValue].FirstOrDefault();
            }

            return result;
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
                var result = Bind(bindingContext);

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
