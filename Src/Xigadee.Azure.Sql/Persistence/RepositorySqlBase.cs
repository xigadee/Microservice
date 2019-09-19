using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to create a SQL based repository.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public abstract class RepositorySqlBase<K, E> : RepositoryBase<K, E>
        where K : IEquatable<K>
    {
        #region Declarations
        /// <summary>
        /// The stored procedure namer class.
        /// </summary>
        protected ISqlStoredProcedureResolver SpNamer { get; }
        /// <summary>
        /// The SQL connection string.
        /// </summary>
        protected readonly string _sqlConnection;
        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositorySqlBase{K, E}"/> class.
        /// </summary>
        /// <param name="sqlConnection">The SQL connection string.</param>
        /// <param name="keyMaker">The key maker.</param>
        /// <param name="spNamer">The stored procedure namer class.</param>
        /// <param name="referenceMaker">The optional entity reference maker.</param>
        /// <param name="propertiesMaker">The optional entity properties maker.</param>
        /// <param name="versionPolicy">The version policy.</param>
        /// <param name="keyManager">The key manager.</param>
        /// <exception cref="ArgumentNullException">The sqlConnection cannot be null.</exception>
        protected RepositorySqlBase(string sqlConnection
            , Func<E, K> keyMaker = null
            , ISqlStoredProcedureResolver spNamer = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<E> versionPolicy = null
            , RepositoryKeyManager<K> keyManager = null
            )
            : base(keyMaker, referenceMaker, propertiesMaker, versionPolicy, keyManager)
        {
            _sqlConnection = sqlConnection ?? throw new ArgumentNullException("sqlConnection");

            SpNamer = spNamer ?? new SqlStoredProcedureResolver<E>();
        }
        #endregion

        #region Create
        /// <summary>
        /// Implements the internal SQL create logic.
        /// </summary>
        protected override async Task<RepositoryHolder<K, E>> CreateInternal(K key, E entity, RepositorySettings options
            , Action<RepositoryHolder<K, E>> holderAction)
        {
            var ctx = new SqlEntityContext<K, E>(SpNamer.StoredProcedureCreate, options, key, entity);

            VersionPolicySet(ctx, false);

            await ExecuteSqlCommand(ctx
                , DbSerializeEntity
                , DbDeserializeEntity
                );

            if (ctx.IsSuccessResponse)
                ctx.ResponseEntities.Add(ctx.EntityOutgoing);

            return ProcessOutputEntity(ctx, holderAction);
        }
        #endregion
        #region Read
        /// <summary>
        /// Read the entity from the SQL server
        /// </summary>
        protected override async Task<RepositoryHolder<K, E>> ReadInternal(K key, RepositorySettings options
            , Action<RepositoryHolder<K, E>> holderAction)
        {
            var ctx = new SqlEntityContext<K, E>(SpNamer.StoredProcedureRead, options, key);

            await ExecuteSqlCommand(ctx
                , DbSerializeKey
                , DbDeserializeEntity
                );

            return ProcessOutputEntity(ctx, holderAction);
        }
        /// <summary>
        /// Read the entity from the SQL server by reference.
        /// </summary>
        protected override async Task<RepositoryHolder<K, E>> ReadByRefInternal(string refKey, string refValue, RepositorySettings options
            , Action<RepositoryHolder<K, E>> holderAction)
        {
            var ctx = new SqlEntityContext<K, E>(SpNamer.StoredProcedureReadByRef, options);
            ctx.Reference = (refKey, refValue);

            await ExecuteSqlCommand(ctx
                , DbSerializeKeyReference
                , DbDeserializeEntity
                );

            return ProcessOutputEntity(ctx, holderAction);
        }
        #endregion
        #region Update
        /// <summary>
        /// Updates the entity.
        /// </summary>
        protected override async Task<RepositoryHolder<K, E>> UpdateInternal(K key, E entity, RepositorySettings options
            , Action<RepositoryHolder<K, E>> holderAction)
        {
            var ctx = new SqlEntityContext<K, E>(SpNamer.StoredProcedureUpdate, options, key, entity);

            VersionPolicySet(ctx, true);

            await ExecuteSqlCommand(ctx
                , DbSerializeEntity
                , DbDeserializeEntity
                );

            if (ctx.IsSuccessResponse)
                ctx.ResponseEntities.Add(ctx.EntityOutgoing);

            return ProcessOutputEntity(ctx, holderAction);
        }
        #endregion
        #region Delete
        /// <summary>
        /// Deletes the entity from the SQL server, is supported.
        /// </summary>
        protected override async Task<RepositoryHolder<K, Tuple<K, string>>> DeleteInternal(K key, RepositorySettings options
            , Action<RepositoryHolder<K, Tuple<K, string>>> holderAction)
        {
            var ctx = new SqlEntityContext<K, Tuple<K, string>>(SpNamer.StoredProcedureDelete, options, key);

            await ExecuteSqlCommand(ctx
                , DbSerializeKey
                , DbDeserializeVersion
                );

            return ProcessOutputVersion(ctx, key, holderAction);
        }
        /// <summary>
        /// Delete the entity by reference
        /// </summary>
        protected override async Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRefInternal(string refKey, string refValue, RepositorySettings options
            , Action<RepositoryHolder<K, Tuple<K, string>>> holderAction)
        {
            var ctx = new SqlEntityContext<K, Tuple<K, string>>(SpNamer.StoredProcedureDeleteByRef, options);
            ctx.Reference = (refKey, refValue);

            await ExecuteSqlCommand(ctx
                , DbSerializeKeyReference
                , DbDeserializeVersion
                );

            return ProcessOutputVersion(ctx, onEvent: holderAction);
        }
        #endregion
        #region Version
        /// <summary>
        /// Retrieves the entity version.
        /// </summary>
        protected override async Task<RepositoryHolder<K, Tuple<K, string>>> VersionInternal(K key, RepositorySettings options
            , Action<RepositoryHolder<K, Tuple<K, string>>> holderAction)
        {
            var ctx = new SqlEntityContext<K, Tuple<K, string>>(SpNamer.StoredProcedureVersion, options, key);

            await ExecuteSqlCommand(ctx
                , DbSerializeKey
                , DbDeserializeVersion
                );

            return ProcessOutputVersion(ctx, key, holderAction);
        }

        /// <summary>
        /// Returns the entity version by reference.
        /// </summary>
        protected override async Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRefInternal(string refKey, string refValue
            , RepositorySettings options, Action<RepositoryHolder<K, Tuple<K, string>>> holderAction)
        {
            var ctx = new SqlEntityContext<K, Tuple<K, string>>(SpNamer.StoredProcedureVersionByRef, options);
            ctx.Reference = (refKey, refValue);

            await ExecuteSqlCommand(ctx
                , DbSerializeKeyReference
                , DbDeserializeVersion
                );

            return ProcessOutputVersion(ctx, onEvent: holderAction);
        }
        #endregion

        #region Search
        /// <summary>
        /// Searches the entity store.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public override async Task<RepositoryHolder<SearchRequest, SearchResponse>> Search(SearchRequest rq, RepositorySettings options = null)
        {
            OnBeforeSearchEvent(rq);

            var response = new SearchResponse();
            response.PopulateSearchRequest(rq);

            var ctx = new SqlEntityContext<SearchRequest, SearchResponse>(SearchSpName(rq.Id), options, rq);
            ctx.EntityOutgoing = response;

            await ExecuteSqlCommand(ctx
                , DbSerializeSearchRequest
                , DbDeserializeSearchResponse
                );

            var rs = new RepositoryHolder<SearchRequest, SearchResponse>(rq, null, ctx.EntityOutgoing, ctx.ResponseCode, ctx.ResponseMessage);

            OnAfterSearchEvent(rs);

            return rs;        
        }
        #endregion
        #region SearchEntity
        /// <summary>
        /// Searches the entity store.
        /// </summary>
        /// <param name="rq">The request.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and entities.
        /// </returns>
        public override async Task<RepositoryHolder<SearchRequest, SearchResponse<E>>> SearchEntity(SearchRequest rq, RepositorySettings options = null)
        {
            OnBeforeSearchEvent(rq);

            var response = new SearchResponse<E>();
            response.PopulateSearchRequest(rq);

            var ctx = new SqlEntityContext<SearchRequest, SearchResponse<E>>(SearchEntitySpName(rq.Id), options, rq);
            ctx.EntityOutgoing = response;

            await ExecuteSqlCommand(ctx
                , DbSerializeSearchRequestEntity
                , DbDeserializeSearchResponseEntity
                );

            var rs = new RepositoryHolder<SearchRequest, SearchResponse<E>>(rq, null, ctx.EntityOutgoing, ctx.ResponseCode, ctx.ResponseMessage);

            OnAfterSearchEntityEvent(rs);

            return rs;
        }
        #endregion

        /// <summary>
        /// This returns the name of the search stored procedure.
        /// </summary>
        /// <param name="id">The search type. This will be set to Default if left as null.</param>
        /// <returns>Returns the stored procedure name.</returns>
        protected virtual string SearchSpName(string id) => SpNamer.StoredProcedureSearch(id ?? "Default");
        /// <summary>
        /// This returns the name of the search entity stored procedure.
        /// </summary>
        /// <param name="id">The search type. This will be set to Default if left as null.</param>
        /// <returns>Returns the stored procedure name.</returns>
        protected virtual string SearchEntitySpName(string id) => SpNamer.StoredProcedureSearchEntity(id ?? "Default");

        /// <summary>
        /// This method serializes the entity in to the SqlCommand.
        /// </summary>
        /// <param name="ctx">The context</param>
        protected virtual void DbSerializeSearchRequest(SqlEntityContext<SearchRequest, SearchResponse> ctx) => DbSerializeSearchRequestCombined(ctx);

        /// <summary>
        /// This method serializes the entity in to the SqlCommand.
        /// </summary>
        /// <param name="ctx">The context.</param>
        protected virtual void DbSerializeSearchRequestEntity(SqlEntityContext<SearchRequest, SearchResponse<E>> ctx) => DbSerializeSearchRequestCombined(ctx);

        /// <summary>
        /// This is the combined search request.
        /// </summary>
        /// <param name="ctx">The context.</param>
        protected abstract void DbSerializeSearchRequestCombined(ISqlEntityContextKey<SearchRequest> ctx);

        /// <summary>
        /// This method deserializes the entity in to the SqlCommand.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="ctx">The context</param>
        protected abstract void DbDeserializeSearchResponse(SqlDataReader dataReader, SqlEntityContext<SearchRequest, SearchResponse> ctx);

        /// <summary>
        /// This method deserializes the entity in to the SqlCommand.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="ctx">The context</param>
        protected abstract void DbDeserializeSearchResponseEntity(SqlDataReader dataReader, SqlEntityContext<SearchRequest, SearchResponse<E>> ctx);

        #region DbSerializeKey(ISqlEntityContextKey<K> ctx)
        /// <summary>
        /// This method serializes the entity key in to the SQL command.
        /// </summary>
        /// <param name="ctx">The context.</param>
        protected virtual void DbSerializeKey(ISqlEntityContextKey<K> ctx)
        {
            if (typeof(K) == typeof(Guid))
                ctx.Command.Parameters.Add("@ExternalId", SqlDbType.UniqueIdentifier).Value = ctx.Key;
            else if (typeof(K) == typeof(string))
                ctx.Command.Parameters.Add("@ExternalId", SqlDbType.NVarChar, 255).Value = ctx.Key;
            else if (typeof(K) == typeof(long))
                ctx.Command.Parameters.Add("@ExternalId", SqlDbType.BigInt).Value = ctx.Key;
            else if (typeof(K) == typeof(int))
                ctx.Command.Parameters.Add("@ExternalId", SqlDbType.Int).Value = ctx.Key;
            else
                throw new NotSupportedException($"Key type '{typeof(K).Name}' is not supported automatically. Override DbSerializeKey");
        }
        #endregion
        #region DbSerializeKeyReference(SqlEntityContext ctx)
        /// <summary>
        /// This method serializes the entity key in to the SQL command.
        /// </summary>
        /// <param name="ctx">The context.</param>
        protected virtual void DbSerializeKeyReference(SqlEntityContext ctx)
        {
            ctx.Command.Parameters.Add("@RefType", SqlDbType.NVarChar, 50).Value = ctx.Reference.type;
            ctx.Command.Parameters.Add("@RefValue", SqlDbType.NVarChar, 255).Value = ctx.Reference.value;
        }
        #endregion

        #region DbDeserializeVersion(SqlDataReader dataReader)
        /// <summary>
        /// This method deserializes a data reader record into a version tuple.
        /// </summary>
        /// <param name="dataReader">The incoming data reader class.</param>
        /// <param name="ctx">The context.</param>
        protected virtual void DbDeserializeVersion(SqlDataReader dataReader, SqlEntityContext<Tuple<K, string>> ctx)
        {
            var key = KeyManager.Deserialize(dataReader["ExternalId"].ToString());
            var versionId = dataReader["VersionId"].ToString();

            //DataTable schema = dataReader.GetSchemaTable();
            //string versionId = schema?.Columns.Contains("VersionId") ?? false ? dataReader["VersionId"].ToString() : null;

            ctx.ResponseEntities.Add(new Tuple<K, string>(key, versionId));
        }
        #endregion

        #region ExecuteSqlCommand<ET> ...
        /// <summary>
        /// Executes a SQL command and deserializes the response into a set of entities.
        /// </summary>
        /// <typeparam name="KT">The key type.</typeparam>
        /// <typeparam name="ET">The entity type.</typeparam>
        /// <param name="ctx">The context.</param>
        /// <param name="populateRequest">Populate command i.e. add SQL parameters</param>
        /// <param name="processResponse">Read an entity out from a SQL Data Reader Record</param>
        /// <returns>Returns a response object with a set of returned entities.</returns>
        protected async Task ExecuteSqlCommand<KT,ET>(SqlEntityContext<KT,ET> ctx
            , Action<SqlEntityContext<KT, ET>> populateRequest
            , Action<SqlDataReader, SqlEntityContext<KT, ET>> processResponse = null) 
            where KT : IEquatable<KT>
        {
            SqlParameter paramReturnValue = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(_sqlConnection))
                {
                    cn.Open();
                    ctx.Command = new SqlCommand(ctx.SpName)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Connection = cn
                    };

                    populateRequest(ctx);

                    paramReturnValue = new SqlParameter("@return_value", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
                    ctx.Command.Parameters.Add(paramReturnValue);

                    try
                    {
                        using (var reader = await ctx.Command.ExecuteReaderAsync())
                        {
                            while (processResponse != null && await reader.ReadAsync())
                            {
                                try
                                {
                                    processResponse(reader, ctx);
                                }
                                catch (Exception ex)
                                {
                                    if (!SqlErrorsCheck(reader, ctx, ex))
                                        throw;
                                }
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        if (paramReturnValue.Value == null || (int)paramReturnValue.Value == 0)
                            throw;
                    }

                    if (paramReturnValue.Value != null)
                        ctx.ResponseCode = (int)paramReturnValue.Value;
                }
            }
            catch (Exception ex)
            {
                if (paramReturnValue?.Value != null)
                    ctx.ResponseCode = (int)paramReturnValue.Value;
                else
                {
                    ctx.ResponseCode = 500;
                }
                //Moved exception error message to be obvious.
                ctx.ResponseMessage = ex.Message;

                throw;
            }
        }

        /// <summary>
        /// This method checks whether the executed request has generated any SQL errors.
        /// </summary>
        /// <typeparam name="ET">The entity type to return.</typeparam>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="response">The response class.</param>
        /// <param name="ex">The exception thrown.</param>
        /// <returns>Returns true if errors are detected.</returns>
        protected virtual bool SqlErrorsCheck<ET>(SqlDataReader dataReader, SqlEntityContext<ET> response, Exception ex = null)
        {
            var columnSchema = dataReader.GetColumnSchema();
            if (columnSchema.Any(dbc => dbc.ColumnName.Equals("ErrorNumber")))
            {
                response.ResponseMessage = dataReader["ErrorNumber"].ToString();
                response.ResponseCode = "2601".Equals(response.ResponseMessage) ? 430 : 500;
            }

            if (columnSchema.Any(dbc => dbc.ColumnName.Equals("ErrorMessage")))
                response.ResponseMessage = $"{response.ResponseMessage}-{dataReader["ErrorMessage"]}";

            return !string.IsNullOrEmpty(response.ResponseMessage);
        }
        #endregion

        #region ProcessOutputEntity/ProcessOutputVersion
        /// <summary>
        /// Converts the SQL output to a repository holder format..
        /// </summary>
        /// <param name="sqlResponse">The SQL response.</param>
        /// <param name="onEvent">The event to fire.</param>
        /// <returns>The repository response.</returns>
        protected virtual RepositoryHolder<K, E> ProcessOutputEntity(SqlEntityContext<E> sqlResponse
            , Action<RepositoryHolder<K, E>> onEvent = null)
        {
            E entity = sqlResponse.ResponseEntities.FirstOrDefault();
            K key = entity != null ? KeyMaker(entity) : default(K);

            var rs = new RepositoryHolder<K, E>(key, null, entity, sqlResponse.ResponseCode, sqlResponse.ResponseMessage);

            onEvent?.Invoke(rs);

            return rs;
        }

        /// <summary>
        /// Converts the SQL output to a repository holder format..
        /// </summary>
        /// <param name="sqlResponse">The SQL response.</param>
        /// <param name="key">The optional key.</param>
        /// <returns>The repository response.</returns>
        protected virtual RepositoryHolder<K, Tuple<K, string>> ProcessOutputVersion(
            SqlEntityContext<Tuple<K, string>> sqlResponse, K key = default(K)
            , Action<RepositoryHolder<K, Tuple<K, string>>> onEvent = null)

        {
            var entity = sqlResponse.ResponseEntities.FirstOrDefault();
            var rs = (entity == null) ? new RepositoryHolder<K, Tuple<K, string>>(key, null, null, 404)
                : new RepositoryHolder<K, Tuple<K, string>>(entity.Item1, null, new Tuple<K, string>(entity.Item1, entity.Item2), sqlResponse.ResponseCode, sqlResponse.ResponseMessage);

            onEvent?.Invoke(rs);

            return rs;
        }
        #endregion

        #region VersionPolicySet(SqlEntityContext<K, E> ctx, bool isUpdate)
        /// <summary>
        /// This method sets the version policy for the specific entity.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="isUpdate">Specifies whether this is an update/</param>
        protected virtual void VersionPolicySet(SqlEntityContext<K, E> ctx, bool isUpdate)
        {
            var entity = ctx.EntityIncoming;

            ctx.EntityOutgoing = JsonHelper.Clone(entity);

            //OK, do we have to update the version id?
            if (isUpdate && (VersionPolicy?.SupportsOptimisticLocking ?? false))
            {
                var incomingVersionId = VersionPolicy.EntityVersionAsString(entity);
                string newVersion = VersionPolicy.EntityVersionUpdate(ctx.EntityOutgoing);
            }
        }
        #endregion

        #region DbSerializeEntity
        /// <summary>
        /// This method serializes the entity in to the SqlCommand.
        /// </summary>
        /// <param name="ctx">The context</param>
        protected abstract void DbSerializeEntity(SqlEntityContext<E> ctx);
        #endregion
        #region DbDeserializeEntity
        /// <summary>
        /// This method deserializes a data reader record into an entity.
        /// </summary>
        /// <param name="dataReader">Data reader</param>
        /// <param name="ctx">The context</param>
        protected abstract void DbDeserializeEntity(SqlDataReader dataReader, SqlEntityContext<E> ctx); 
        #endregion
    }
}
