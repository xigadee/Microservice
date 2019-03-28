using Microsoft.AspNetCore.Mvc;

namespace Xigadee
{
    /// <summary>
    /// This model allows the GET verb to be combined with both a traditional entity read and a search request.
    /// </summary>
    [ModelBinder(typeof(CombinedRequestModelBinder))]
    public class CombinedRequestModel
    {
        public CombinedRequestModelType? Type { get; set; }
        /// <summary>
        /// Gets or sets the entity request model.
        /// </summary>
        public EntityRequestModel Erm { get; set; }
        /// <summary>
        /// Gets or sets the search request model.
        /// </summary>
        public SearchRequestModel Srm { get; set; }
    }

    public enum CombinedRequestModelType
    {
        ReadEntity,
        Search,
        SearchEntity
    }
}
