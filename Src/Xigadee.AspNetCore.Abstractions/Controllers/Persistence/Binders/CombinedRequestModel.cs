using Microsoft.AspNetCore.Mvc;

namespace Xigadee
{
    /// <summary>
    /// This model allows the GET verb to be combined with both a traditional entity read and a search request.
    /// </summary>
    [ModelBinder(typeof(CombinedRequestModelBinder))]
    public class CombinedRequestModel
    {
        /// <summary>
        /// Gets or sets the request type.
        /// </summary>
        public CombinedRequestModelType? Type { get; set; }
        /// <summary>
        /// Gets or sets the entity request model.
        /// </summary>
        public EntityRequestModel EntityRequest { get; set; }
        /// <summary>
        /// Gets or sets the search request model.
        /// </summary>
        public SearchRequestModel SearchRequest { get; set; }
    }

    /// <summary>
    /// The GET request type.
    /// </summary>
    public enum CombinedRequestModelType
    {
        /// <summary>
        /// Read an entity
        /// </summary>
        ReadEntity,
        /// <summary>
        /// The search for a set of entity parameters.
        /// </summary>
        Search,
        /// <summary>
        /// The search for a set of entities.
        /// </summary>
        SearchEntity
    }
}
