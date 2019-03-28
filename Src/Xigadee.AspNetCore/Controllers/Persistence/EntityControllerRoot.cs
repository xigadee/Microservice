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
    public abstract class EntityControllerRoot<K, E> : ControllerBase
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
        protected EntityControllerRoot(ILogger logger, IRepositoryAsync<K, E> repository, RepositoryKeyManager<K> keyManager = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

            _keyManager = keyManager ?? RepositoryKeyManager.Resolve<K>(); 
        }
        #endregion

        #region EntityHeadersAdd(RepositoryHolder<K, E> rs)
        /// <summary>
        /// Adds the entity id to the outgoing headers.
        /// </summary>
        /// <param name="rs">The repository response.</param>
        protected virtual void EntityHeadersAdd(RepositoryHolder<K, E> rs)
        {
            var t = rs.KeyReference;

            if (!string.IsNullOrEmpty(t?.Item1))
                Response.Headers.Add("X-EntityId", t.Item1);

            if (!string.IsNullOrEmpty(t?.Item2))
                Response.Headers.Add("X-VersionId", t.Item2);

            // Store entity id 
            if (Activity.Current != null)
                Activity.Current.AddBaggage("EntityId", $"{rs.Key}");
        }
        #endregion

        #region CreateRoot([FromBody]E rq)
        /// <summary>
        /// Creates an entity.
        /// </summary>
        /// <param name="rq">The entity to create.</param>
        /// <returns>Returns response containing the created entity and status code</returns>
        protected virtual async Task<IActionResult> CreateRoot([FromBody]E rq)
        {
            K key = default(K);

            try
            {
                if (rq == null)
                    return StatusCode(StatusCodes.Status400BadRequest);

                var rs = await _repository.Create(rq);

                if (rs.IsSuccess)
                {
                    EntityHeadersAdd(rs);
                    return StatusCode(rs.ResponseCode, rs.Entity);
                }

                return StatusCode(rs.ResponseCode);
            }
            catch (HttpStatusOutputException stex)
            {
                stex.Log(_logger, (ex) => $"{typeof(E).Name} create error: {key} {stex.Message}");
                return StatusCode(stex.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{typeof(E).Name} create error: {key}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion
        #region UpdateRoot([FromBody]E rq)
        /// <summary>
        /// Updates an entity.
        /// </summary>
        /// <param name="rq">The updated entity.</param>
        /// <returns>Returns response containing status code</returns>
        protected virtual async Task<IActionResult> UpdateRoot([FromBody]E rq)
        {
            K key = default(K);

            try
            {
                //Do we have an entity?
                if (rq == null)
                    return StatusCode(StatusCodes.Status400BadRequest);

                var rs = await _repository.Update(rq);

                if (rs.IsSuccess)
                {
                    EntityHeadersAdd(rs);
                    return StatusCode(rs.ResponseCode, rs.Entity);
                }

                return StatusCode(rs.ResponseCode);
            }
            catch (HttpStatusOutputException stex)
            {
                stex.Log(_logger, (ex) => $"{typeof(E).Name} update error: {key} {ex?.Message}");
                return StatusCode(stex.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{typeof(E).Name} update error: {key}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region ReadRoot(EntityRequestModel input)
        /// <summary>
        /// Retrieves an entity by id or ref type/value or search for entities.
        /// </summary>
        /// <param name="input">The entity id or ref type/value or search parameters.</param>
        /// <returns>Returns matching entity/entities</returns>
        protected virtual async Task<IActionResult> ReadRoot(EntityRequestModel input)
        {
            try
            {
                if (!(input?.IsValid ?? false))
                    return StatusCode(StatusCodes.Status400BadRequest);

                //OK, we are looking for a specific entity.
                RepositoryHolder<K, E> rs;

                if (input.IsByKey)
                {
                    var keyRs = _keyManager.TryDeserialize(input.Id);
                    if (!keyRs.success)
                        return StatusCode(StatusCodes.Status400BadRequest);

                    rs = await _repository.Read(keyRs.key);
                }
                else if (input.IsByReference)
                {
                    rs = await _repository.ReadByRef(input.Reftype, input.Refvalue);
                }
                else
                    return StatusCode(StatusCodes.Status400BadRequest);

                if (rs.IsSuccess)
                {
                    EntityHeadersAdd(rs);
                    return StatusCode(rs.ResponseCode, rs.Entity);
                }

                return StatusCode(rs.ResponseCode);
            }
            catch (HttpStatusOutputException stex)
            {
                stex.Log(_logger, (ex) => $"{typeof(E).Name} read or search error: {input} - {stex.Message}");
                return StatusCode(stex.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{typeof(E).Name} read or search uncaught exception: {input} - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region DeleteRoot(EntityRequestModel input)
        /// <summary>
        /// Deletes an entity by id or ref type/value.
        /// </summary>
        /// <param name="input">The entity id or ref type/value.</param>
        /// <returns>Returns response containing status code</returns>
        protected virtual async Task<IActionResult> DeleteRoot(EntityRequestModel input)
        {
            try
            {
                if (!input?.IsValid ?? false)
                    return StatusCode(StatusCodes.Status400BadRequest);

                RepositoryHolder<K, Tuple<K, string>> rs;
                if (input.IsByKey)
                {
                    var keyRs = _keyManager.TryDeserialize(input.Id);
                    if (!keyRs.success)
                        return StatusCode(StatusCodes.Status400BadRequest);

                    rs = await _repository.Delete(keyRs.key);
                }
                else if (input.IsByReference)
                {
                    rs = await _repository.DeleteByRef(input.Reftype, input.Refvalue);
                }
                else
                    return StatusCode(StatusCodes.Status404NotFound);

                return StatusCode(rs.ResponseCode);
            }
            catch (HttpStatusOutputException stex)
            {
                stex.Log(_logger, (ex) => $"{typeof(E).Name} delete error: {input} - {ex.Message}");
                return StatusCode(stex.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{typeof(E).Name} delete error: {input}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion
        #region VersionRoot(EntityRequestModel input)
        /// <summary>
        /// Returns headers for retrieve entity by id or ref type/value or search for entities.
        /// </summary>
        /// <param name="input">The entity id or ref type/value or search parameters.</param>
        /// <returns>Returns matching entity/entities</returns>
        protected virtual async Task<IActionResult> VersionRoot(EntityRequestModel input)
        {
            try
            {
                if (!input?.IsValid ?? false)
                    return StatusCode(StatusCodes.Status400BadRequest);

                RepositoryHolder<K, Tuple<K, string>> rs;
                if (input.IsByKey)
                {
                    var keyRs = _keyManager.TryDeserialize(input.Id);
                    if (!keyRs.success)
                        return StatusCode(StatusCodes.Status400BadRequest);

                    rs = await _repository.Version(keyRs.key);
                }
                else if (input.IsByReference)
                {
                    rs = await _repository.VersionByRef(input.Reftype, input.Refvalue);
                }
                else
                    return StatusCode(StatusCodes.Status404NotFound);

                return StatusCode(rs.ResponseCode);
            }
            catch (HttpStatusOutputException stex)
            {
                stex.Log(_logger, (ex) => $"{typeof(E).Name} version error: {input} - {ex.Message}");
                return StatusCode(stex.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{typeof(E).Name} version error: {input}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion

        #region SearchRoot(SearchRequestModel input)
        /// <summary>
        /// Retrieves an entity by id or ref type/value or search for entities.
        /// </summary>
        /// <param name="input">The entity id or ref type/value or search parameters.</param>
        /// <returns>Returns matching entity/entities</returns>
        protected virtual async Task<IActionResult> SearchRoot(SearchRequestModel input)
        {
            try
            {
                var rs = await _repository.Search(input);

                if (rs.IsSuccess)
                {
                    return StatusCode(rs.ResponseCode, rs.Entity);
                }

                return StatusCode(rs.ResponseCode);
            }
            catch (HttpStatusOutputException stex)
            {
                stex.Log(_logger, (ex) => $"{typeof(E).Name} search error: {input} - {stex.Message}");
                return StatusCode(stex.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{typeof(E).Name} search uncaught exception: {input} - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion
        #region SearchEntityRoot(SearchRequestModel input)
        /// <summary>
        /// Retrieves an entity by id or ref type/value or search for entities.
        /// </summary>
        /// <param name="input">The entity id or ref type/value or search parameters.</param>
        /// <returns>Returns matching entity/entities</returns>
        protected virtual async Task<IActionResult> SearchEntityRoot(SearchRequestModel input)
        {
            try
            {
                var rs = await _repository.SearchEntity(input);

                if (rs.IsSuccess)
                {
                    return StatusCode(rs.ResponseCode, rs.Entity);
                }

                return StatusCode(rs.ResponseCode);
            }
            catch (HttpStatusOutputException stex)
            {
                stex.Log(_logger, (ex) => $"{typeof(E).Name} search error: {input} - {stex.Message}");
                return StatusCode(stex.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{typeof(E).Name} search uncaught exception: {input} - {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        #endregion
    }
}
