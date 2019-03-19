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
    /// <seealso cref="Xigadee.RepositoryBase{K, E}" />
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
            , Func<E, K> keyMaker
            , ISqlStoredProcedureResolver spNamer = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<E, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<E> versionPolicy = null
            , RepositoryKeyManager<K> keyManager = null
            )
            : base(keyMaker, referenceMaker, propertiesMaker, versionPolicy, keyManager)
        {
            _sqlConnection = sqlConnection ?? throw new ArgumentNullException("sqlConnection");

            SpNamer = SpNamer ?? new SqlStoredProcedureResolver<E>();
        } 
        #endregion


        /// <summary>
        /// This method serializes the entity key in to the SQL command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cmd">The command.</param>
        protected abstract void DbSerializeKey(K key, SqlCommand cmd);

        /// <summary>
        /// This method serializes the entity key in to the SQL command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cmd">The command.</param>
        protected virtual void DbSerializeKeyReference(Tuple<string, string> key, SqlCommand cmd)
        {
            cmd.Parameters.Add("@RefType", SqlDbType.NVarChar, 50).Value = key.Item1;
            cmd.Parameters.Add("@RefValue", SqlDbType.NVarChar, 255).Value = key.Item2;
        }

        /// <summary>
        /// This method serializes the entity in to the SqlCommand.
        /// </summary>
        /// <param name="entity">The entity</param>
        /// <param name="cmd">The SQL command.</param>
        protected abstract void DbSerializeEntity(E entity, SqlCommand cmd);


        /// <summary>
        /// This method deserializes a data reader record into an entity.
        /// </summary>
        /// <param name="dataReader">Data reader</param>
        protected abstract E DbDeserializeEntity(SqlDataReader dataReader);

        /// <summary>
        /// This method deserializes a data reader record into a version tuple.
        /// </summary>
        /// <param name="dataReader">The incoming data reader class.</param>
        protected virtual Tuple<K, string> DbDeserializeVersion(SqlDataReader dataReader)
        {
            var key = KeyManager.Deserialize(dataReader["Id"].ToString());
            DataTable schema = dataReader.GetSchemaTable();
            string versionId = schema?.Columns.Contains("VersionId") ?? false ? dataReader["VersionId"].ToString() : null;

            return new Tuple<K, string>(key, versionId);
        }

        #region Create
        /// <summary>
        /// Creates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public override async Task<RepositoryHolder<K, E>> Create(E entity, RepositorySettings options = null)
        {
            OnBeforeCreateEvent(entity);

            var rs = await ExecuteSqlCommand(SpNamer.StoredProcedureCreate, sqlCmd => DbSerializeEntity(entity, sqlCmd), DbDeserializeEntity);

            return ProcessOutputEntity(rs, o => OnAfterCreateEvent(o));
        } 
        #endregion
        #region Read
        /// <summary>
        /// Reads the entity by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public override async Task<RepositoryHolder<K, E>> Read(K key, RepositorySettings options = null)
        {
            OnKeyEvent(KeyEventType.BeforeRead, key);

            var rs = await ExecuteSqlCommand(SpNamer.StoredProcedureRead, sqlCmd => DbSerializeKey(key, sqlCmd), DbDeserializeEntity);

            return ProcessOutputEntity(rs, o => OnAfterReadEvent(o));
        }
        /// <summary>
        /// Reads the entity by a reference key-value pair.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public override async Task<RepositoryHolder<K, E>> ReadByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            OnKeyEvent(KeyEventType.BeforeRead, default(K), refKey,  refValue);

            var rs = await ExecuteSqlCommand(SpNamer.StoredProcedureReadByRef, sqlCmd => DbSerializeKeyReference(new Tuple<string, string>(refKey, refValue), sqlCmd), DbDeserializeEntity);

            return ProcessOutputEntity(rs, o => OnAfterReadEvent(o));
        } 
        #endregion
        #region Update
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public override async Task<RepositoryHolder<K, E>> Update(E entity, RepositorySettings options = null)
        {
            OnBeforeUpdateEvent(entity);

            var rs = await ExecuteSqlCommand(SpNamer.StoredProcedureUpdate, sqlCmd => DbSerializeEntity(entity, sqlCmd), DbDeserializeEntity);

            return ProcessOutputEntity(rs, o => OnAfterUpdateEvent(o));
        } 
        #endregion
        #region Delete
        /// <summary>
        /// Deletes the entity by the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public override async Task<RepositoryHolder<K, Tuple<K, string>>> Delete(K key, RepositorySettings options = null)
        {
            var rs = await ExecuteSqlCommand(SpNamer.StoredProcedureDelete, sqlCmd => DbSerializeKey(key, sqlCmd), DbDeserializeVersion);

            return ProcessOutputVersion(rs);
        }

        /// <summary>
        /// Deletes the entity by reference.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public override async Task<RepositoryHolder<K, Tuple<K, string>>> DeleteByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            var rs = await ExecuteSqlCommand(SpNamer.StoredProcedureDeleteByRef, sqlCmd => DbSerializeKeyReference(new Tuple<string, string>(refKey, refValue), sqlCmd), DbDeserializeVersion);
            return ProcessOutputVersion(rs);
        } 
        #endregion

        #region Version
        /// <summary>
        /// Validates the version by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public override async Task<RepositoryHolder<K, Tuple<K, string>>> Version(K key, RepositorySettings options = null)
        {
            var rs = await ExecuteSqlCommand(SpNamer.StoredProcedureVersion, sqlCmd => DbSerializeKey(key, sqlCmd), DbDeserializeVersion);
            return ProcessOutputVersion(rs);
        }

        /// <summary>
        /// Validates the version by reference.
        /// </summary>
        /// <param name="refKey">The reference key.</param>
        /// <param name="refValue">The reference value.</param>
        /// <param name="options">The repository options.</param>
        /// <returns>
        /// Returns the holder with the response and data.
        /// </returns>
        public override async Task<RepositoryHolder<K, Tuple<K, string>>> VersionByRef(string refKey, string refValue, RepositorySettings options = null)
        {
            var rs = await ExecuteSqlCommand(SpNamer.StoredProcedureVersionByRef, sqlCmd => DbSerializeKeyReference(new Tuple<string, string>(refKey, refValue), sqlCmd), DbDeserializeVersion);
            return ProcessOutputVersion(rs);
        }
        #endregion

        #region ExecuteSqlCommand<ET> ...
        /// <summary>
        /// Executes a SQL command and deserializes the response into a set of entities.
        /// </summary>
        /// <typeparam name="ET">The entity class to return.</typeparam>
        /// <param name="commandName">The SQL stored procedure name</param>
        /// <param name="populateCommand">Populate command i.e. add SQL parameters</param>
        /// <param name="deserializeToEntity">Read an entity out from a SQL Data Reader Record</param>
        /// <returns>Returns a response object with a set of returned entities.</returns>
        protected async Task<SqlEntityResponse<ET>> ExecuteSqlCommand<ET>(string commandName
            , Action<SqlCommand> populateCommand
            , Func<SqlDataReader, ET> deserializeToEntity = null)
        {
            SqlParameter paramReturnValue = null;
            var rs = new SqlEntityResponse<ET>();

            try
            {
                using (SqlConnection cn = new SqlConnection(_sqlConnection))
                {
                    cn.Open();
                    SqlCommand sqlCmd = new SqlCommand(commandName)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Connection = cn
                    };

                    populateCommand(sqlCmd);

                    paramReturnValue = new SqlParameter("@return_value", SqlDbType.Int) { Direction = ParameterDirection.ReturnValue };
                    sqlCmd.Parameters.Add(paramReturnValue);

                    try
                    {
                        using (var reader = await sqlCmd.ExecuteReaderAsync())
                        {
                            while (deserializeToEntity != null && await reader.ReadAsync())
                            {
                                try
                                {
                                    rs.Entities.Add(deserializeToEntity(reader));
                                }
                                catch (Exception)
                                {
                                    if (!SqlErrorsCheck(reader, rs))
                                        throw;

                                    return rs;
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
                        rs.ResponseCode = (int)paramReturnValue.Value;
                }
            }
            catch (Exception ex)
            {
                if (paramReturnValue?.Value != null)
                    rs.ResponseCode = (int)paramReturnValue.Value;
                else
                {
                    rs.ResponseCode = 500;
                    rs.ResponseMessage = ex.Message;
                }

                throw;
            }

            return rs;
        }

        /// <summary>
        /// This method checks whether the executed request has generated any SQL errors.
        /// </summary>
        /// <typeparam name="ET">The entity type to return.</typeparam>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="response">The response class.</param>
        /// <returns>Returns true if errors are detected.</returns>
        protected virtual bool SqlErrorsCheck<ET>(SqlDataReader dataReader, SqlEntityResponse<ET> response)
        {
            var columnSchema = dataReader.GetColumnSchema();
            if (columnSchema.Any(dbc => dbc.ColumnName.Equals("ErrorNumber")))
            {
                response.ResponseMessage = dataReader["ErrorNumber"].ToString();
                response.ResponseCode = "2601".Equals(response.ResponseMessage) ? 409 : 500;
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
        protected virtual RepositoryHolder<K, E> ProcessOutputEntity(SqlEntityResponse<E> sqlResponse
            , Action<RepositoryHolder<K, E>> onEvent = null)
        {
            E entity = sqlResponse.Entities.FirstOrDefault();
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
            SqlEntityResponse<Tuple<K, string>> sqlResponse, K key = default(K)
            , Action<RepositoryHolder<K, Tuple<K, string>>> onEvent = null)

        {
            var entity = sqlResponse.Entities.FirstOrDefault();
            var rs = (entity == null)? new RepositoryHolder<K, Tuple<K, string>>(key, null, null, 404)
                : new RepositoryHolder<K, Tuple<K, string>>(entity.Item1, null, new Tuple<K, string>(entity.Item1, entity.Item2), sqlResponse.ResponseCode,sqlResponse.ResponseMessage);

            onEvent?.Invoke(rs);

            return rs;
        }
        #endregion

        #region Class -> SqlEntityResponse<ET>
        /// <summary>
        /// This class contains the set of entities returned from a SQL command
        /// </summary>
        /// <typeparam name="ET">The entity type.</typeparam>
        protected class SqlEntityResponse<ET>
        {
            /// <summary>
            /// Gets the entity list.
            /// </summary>
            public List<ET> Entities { get; } = new List<ET>();

            /// <summary>
            /// Gets or sets the response code.
            /// </summary>
            public int ResponseCode { get; set; }
            /// <summary>
            /// Gets or sets the optional response message.
            /// </summary>
            public string ResponseMessage { get; set; }
        } 
        #endregion
    }

}
