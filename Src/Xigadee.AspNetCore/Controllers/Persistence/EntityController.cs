using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Xigadee
{
    /// <summary>
    /// This controller can be used to expose a repository through an API endpoint.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class EntityController<K, E> : EntityControllerRoot<K, E>
        where K : IEquatable<K>
    {
        #region Declarations
        /// <summary>
        /// The entity repository.
        /// </summary>
        protected readonly IRepositoryAsync<K, E> _repository;
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly ILogger _logger;
        /// <summary>
        /// The key manager used for serializing and deserializing the key.
        /// </summary>
        protected readonly RepositoryKeyManager<K> _keyManager;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the EntityControllerRoot class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="repository">The underlying repository</param>
        /// <param name="keyManager">The key manager.</param>
        public EntityController(ILogger logger, IRepositoryAsync<K, E> repository, RepositoryKeyManager<K> keyManager = null):base(logger, repository, keyManager)
        {
        }
        #endregion

        #region Create
        /// <summary>
        /// Creates an entity.
        /// </summary>
        /// <param name="rq">The entity to create.</param>
        /// <returns>Returns response containing the created entity and status code</returns>
        [HttpPost()]
        public virtual Task<IActionResult> Create([FromBody]E rq)
        {
            return base.CreateRoot(rq);
        }
        #endregion

        #region Read/Search
        /// <summary>
        /// Retrieves an entity by id or ref type/value or search for entities.
        /// </summary>
        /// <param name="input">The entity id or ref type/value or search parameters.</param>
        /// <returns>Returns matching entity/entities</returns>
        [Route("")]
        [Route("{id1}")]
        [Route("{id1}/{id2}")]
        [HttpGet]
        public virtual Task<IActionResult> Read(CombinedRequestModel input)
        {
            switch (input?.Type)
            {
                case CombinedRequestModelType.ReadEntity:
                    return base.ReadRoot(input.EntityRequest);
                case CombinedRequestModelType.Search:
                    return base.SearchRoot(input.SearchRequest);
                case CombinedRequestModelType.SearchEntity:
                    return base.SearchEntityRoot(input.SearchRequest);
                default:
                    return Task.FromResult((IActionResult)StatusCode(400));
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="rq">The updated entity.</param>
        /// <returns>Returns response containing status code</returns>
        [HttpPut()]
        public virtual Task<IActionResult> Update([FromBody]E rq)
        {
            return base.UpdateRoot(rq);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes an entity by id or ref type/value.
        /// </summary>
        /// <param name="input">The entity id or ref type/value.</param>
        /// <returns>Returns response containing status code</returns>
        [Route("")]
        [Route("{id1}")]
        [HttpDelete]
        public virtual Task<IActionResult> Delete(EntityRequestModel input)
        {
            return base.DeleteRoot(input);
        }
        #endregion
        #region Version
        /// <summary>
        /// Returns headers for retrieve entity by id or ref type/value or search for entities.
        /// </summary>
        /// <param name="input">The entity id or ref type/value or search parameters.</param>
        /// <returns>Returns matching entity/entities</returns>
        [Route("")]
        [Route("{id1}")]
        [HttpHead]
        public virtual Task<IActionResult> Version(EntityRequestModel input)
        {
            return base.VersionRoot(input);
        }
        #endregion
    }
}
