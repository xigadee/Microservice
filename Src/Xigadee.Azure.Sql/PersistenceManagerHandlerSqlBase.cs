#region using
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
#endregion
namespace Xigadee
{
    #region PersistenceManagerHandlerSqlBase<K, E>...
    /// <summary>
    /// This is the default SQL persistence manager for XML based Stored Procedure interaction.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public abstract class PersistenceManagerHandlerSqlBase<K, E> : PersistenceManagerHandlerSqlBase<K, E, PersistenceStatistics>
        where K : IEquatable<K>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor with a manual connection string.
        /// </summary>
        /// <param name="connection">The sql datbase connection.</param>
        /// <param name="keyMaker">The key maker function.</param>
        /// <param name="keyDeserializer"></param>
        /// <param name="persistenceEntitySerializer">The entity serializer for persistence.</param>
        /// <param name="cachingEntitySerializer">The entity serializer for caching</param>
        /// <param name="xmlVersionMaker"></param>
        /// <param name="entityName"></param>
        /// <param name="versionPolicy"></param>
        /// <param name="defaultTimeout"></param>
        /// <param name="persistenceRetryPolicy"></param>
        /// <param name="resourceProfile"></param>
        /// <param name="cacheManager"></param>
        /// <param name="referenceMaker"></param>
        /// <param name="referenceHashMaker"></param>
        /// <param name="keySerializer"></param>
        protected PersistenceManagerHandlerSqlBase(string connection
            , Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , EntitySerializer<E> persistenceEntitySerializer = null
            , EntitySerializer<E> cachingEntitySerializer = null
            , Func<XElement, Tuple<K, string>> xmlVersionMaker = null
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            , Func<K, string> keySerializer = null
            )
            : base(connection
                  , keyMaker
                  , keyDeserializer
                  , persistenceEntitySerializer
                  , cachingEntitySerializer
                  , xmlVersionMaker: xmlVersionMaker
                  , entityName: entityName
                  , versionPolicy: versionPolicy
                  , defaultTimeout: defaultTimeout
                  , persistenceRetryPolicy: persistenceRetryPolicy
                  , resourceProfile: resourceProfile
                  , cacheManager: cacheManager
                  , referenceMaker: referenceMaker
                  , referenceHashMaker: referenceHashMaker
                  , keySerializer: keySerializer
                  )
        {
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// This is the default SQL persistence manager for XML based Stored Procedure interaction.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    /// <typeparam name="S">The extended persistence statistics class.</typeparam>
    public abstract class PersistenceManagerHandlerSqlBase<K, E, S> : PersistenceCommandBase<K, E, S, PersistenceCommandPolicy>
        where K : IEquatable<K>
        where S : PersistenceStatistics, new()
    {
        #region Declaration
        /// <summary>
        /// Serialize the entity for persistence.
        /// </summary>
        protected EntitySerializer<E> mPersistenceEntitySerializer;
        /// <summary>
        /// Serializer the entity for caching.
        /// </summary>
        protected EntitySerializer<E> mCacheEntitySerializer;
        /// <summary>
        /// This function creates the version maker.
        /// </summary>
        protected Func<XElement, Tuple<K, string>> mXmlVersionMaker;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor with a manual connection string.
        /// </summary>
        /// <param name="connection">The sql datbase connection.</param>
        /// <param name="keyMaker">The key maker function.</param>
        /// <param name="persistenceEntitySerializer"></param>
        /// <param name="cachingEntitySerializer"></param>
        /// <param name="xmlVersionMaker"></param>
        /// <param name="keyDeserializer"></param>
        /// <param name="entityName"></param>
        /// <param name="versionPolicy"></param>
        /// <param name="defaultTimeout"></param>
        /// <param name="persistenceRetryPolicy"></param>
        /// <param name="resourceProfile"></param>
        /// <param name="cacheManager"></param>
        /// <param name="referenceMaker"></param>
        /// <param name="keySerializer"></param>
        /// <param name="referenceHashMaker"></param>
        protected PersistenceManagerHandlerSqlBase(string connection
            , Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , EntitySerializer<E> persistenceEntitySerializer = null
            , EntitySerializer<E> cachingEntitySerializer = null
            , Func<XElement, Tuple<K, string>> xmlVersionMaker = null
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<K, string> keySerializer = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            )
            : base(persistenceRetryPolicy: persistenceRetryPolicy
                  , resourceProfile: resourceProfile
                  , cacheManager: cacheManager
                  , defaultTimeout: defaultTimeout
                  , entityName: entityName
                  , versionPolicy: versionPolicy
                  , keyMaker: keyMaker
                  , persistenceEntitySerializer: persistenceEntitySerializer
                  , cachingEntitySerializer: cachingEntitySerializer
                  , keySerializer: keySerializer
                  , keyDeserializer: keyDeserializer
                  , referenceMaker: referenceMaker
                  , referenceHashMaker: referenceHashMaker
                  )
        {
            Connection = connection;
            mPersistenceEntitySerializer = persistenceEntitySerializer;
            mCacheEntitySerializer = cachingEntitySerializer ?? mTransform?.CacheEntitySerializer;
            mXmlVersionMaker = xmlVersionMaker;
        }
        #endregion

        #region Connection
        /// <summary>
        /// This is the SQL connection.
        /// </summary>
        public string Connection { get; set; }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="versionPolicy"></param>
        /// <param name="keyMaker"></param>
        /// <param name="persistenceEntitySerializer"></param>
        /// <param name="cacheEntitySerializer"></param>
        /// <param name="keySerializer"></param>
        /// <param name="keyDeserializer"></param>
        /// <param name="referenceMaker"></param>
        /// <param name="referenceHashMaker"></param>
        /// <returns></returns>
        protected override EntityTransformHolder<K, E> EntityTransformCreate(string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , Func<E, K> keyMaker = null
            , EntitySerializer<E> persistenceEntitySerializer = null
            , EntitySerializer<E> cacheEntitySerializer = null
            , Func<K, string> keySerializer = null
            , Func<string, K> keyDeserializer = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Tuple<string, string>, string> referenceHashMaker = null
            )
        {
            var transform = base.EntityTransformCreate(entityName, versionPolicy, keyMaker, persistenceEntitySerializer, cacheEntitySerializer, keySerializer, keyDeserializer, referenceMaker, referenceHashMaker);

            transform.CacheEntitySerializer = mCacheEntitySerializer ?? new EntitySerializer<E>(transform.JsonSerialize, transform.JsonDeserialize);

            return transform;
        }

        #region Abstract methods

        /// <summary>
        /// This method serializes the entity key in to the Sql command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cmd">The command.</param>
        public abstract void DbSerializeKey(K key, SqlCommand cmd);
        /// <summary>
        /// This method serializes the entity key in to the Sql command.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="cmd">The command.</param>
        public virtual void DbSerializeKeyReference(Tuple<string, string> key, SqlCommand cmd)
        {
            cmd.Parameters.Add("@RefType", SqlDbType.NVarChar, 50).Value = key.Item1;
            cmd.Parameters.Add("@RefValue", SqlDbType.NVarChar, 255).Value = key.Item2;
        }
        /// <summary>
        /// This method serializes the entity in to the SqlCommand.
        /// </summary>
        /// <param Name="entity">The entity</param>
        /// <param Name="cmd">The command.</param>
        public abstract void DbSerializeEntity(E entity, SqlCommand cmd);
        #endregion

        #region Stored procedure names
        /// <summary>
        /// The create stored procedure Name.
        /// </summary>
        protected virtual string StoredProcedureCreate
        {
            get
            {
                return string.Format("External.{0}Create", typeof(E).Name);
            }
        }
        /// <summary>
        /// The read stored procedure Name.
        /// </summary>
        protected virtual string StoredProcedureRead
        {
            get
            {
                return string.Format("External.{0}Read", typeof(E).Name);
            }
        }
        /// <summary>
        /// The read stored procedure Name.
        /// </summary>
        protected virtual string StoredProcedureReadByRef
        {
            get
            {
                return string.Format("External.{0}ReadByRef", typeof(E).Name);
            }
        }
        /// <summary>
        /// The update stored procedure Name.
        /// </summary>
        protected virtual string StoredProcedureUpdate
        {
            get
            {
                return string.Format("External.{0}Update", typeof(E).Name);
            }
        }
        /// <summary>
        /// The delete stored procedure Name.
        /// </summary>
        protected virtual string StoredProcedureDelete
        {
            get
            {
                return string.Format("External.{0}Delete", typeof(E).Name);
            }
        }
        /// <summary>
        /// The delete stored procedure Name.
        /// </summary>
        protected virtual string StoredProcedureDeleteByRef
        {
            get
            {
                return string.Format("External.{0}DeleteByRef", typeof(E).Name);
            }
        }
        /// <summary>
        /// The delete stored procedure Name.
        /// </summary>
        protected virtual string StoredProcedureVersion
        {
            get
            {
                return string.Format("External.{0}Version", typeof(E).Name);
            }
        }
        /// <summary>
        /// The delete stored procedure Name.
        /// </summary>
        protected virtual string StoredProcedureVersionByRef
        {
            get
            {
                return string.Format("External.{0}VersionByRef", typeof(E).Name);
            }
        }
        #endregion

        #region SqlCommandTemplate<KT,ET>...
        /// <summary>
        /// This method is used to read an entity from the system.
        /// </summary>
        /// <param Name="id">The entity key.</param>
        /// <param name="dbConnection"></param>
        /// <param name="commandname"></param>
        /// <param name="act"></param>
        /// <param name="conv"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected async Task<R> SqlCommandTemplateXml<R>(
            string dbConnection,
            string commandname,
            Action<SqlCommand> act)
            where R : PersistenceResponseHolder, new()
        {
            SqlParameter paramReturnValue = null;

            R rs = new R();

            try
            {
                using (SqlConnection cn = new SqlConnection(dbConnection))
                {
                    cn.Open();

                    SqlCommand sqlCmd = new SqlCommand(commandname)
                    {
                        CommandType = CommandType.StoredProcedure,
                        Connection = cn
                    };

                    act(sqlCmd);

                    paramReturnValue = new SqlParameter
                    {
                        ParameterName = "@return_value",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.ReturnValue
                    };

                    sqlCmd.Parameters.Add(paramReturnValue);

                    try
                    {
                        using (XmlReader xmlR = await sqlCmd.ExecuteXmlReaderAsync())
                        {
                            StringBuilder sb = new StringBuilder();

                            if (xmlR != null)
                                while (xmlR.ReadState != ReadState.EndOfFile && xmlR.Read())
                                    sb.AppendLine(xmlR.ReadOuterXml());

                            rs.Content = sb.ToString();
                        }
                    }
                    catch (InvalidOperationException ioex)
                    {
                        if (paramReturnValue.Value == null || (int)paramReturnValue.Value == 0)
                            throw;
                    }

                    if (paramReturnValue.Value != null)
                        rs.StatusCode = (int)paramReturnValue.Value;
                }
            }
            catch (Exception ex)
            {
                if (paramReturnValue?.Value != null)
                    rs.StatusCode = (int) paramReturnValue.Value;
                else
                {
                    rs.StatusCode = 500;
                    rs.Ex = ex;
                }

                throw;
            }

            rs.IsSuccess = rs.StatusCode >= 200 && rs.StatusCode <= 299;

            if (rs.StatusCode == 401)
                Collector?.LogMessage(LoggingLevel.Error, $"Sql DB action {commandname} failed: ", "SQL");

            return rs;
        }
        #endregion

        #region InternalCreate
        protected override async Task<IResponseHolder<E>> InternalCreate(K key, PersistenceRequestHolder<K, E> holder)
        {
            var rs = await SqlCommandTemplateXml<PersistenceResponseHolder<E>>(Connection
                , StoredProcedureCreate
                , sqlCmd => DbSerializeEntity(holder.Rq.Entity, sqlCmd)
                );

            ProcessOutputEntity(holder, rs);

            return rs;
        }
        #endregion
        #region InternalRead
        protected override async Task<IResponseHolder<E>> InternalRead(K key, PersistenceRequestHolder<K, E> holder)
        {
            var rs = await SqlCommandTemplateXml<PersistenceResponseHolder<E>>(Connection
                , StoredProcedureRead
                , sqlCmd => DbSerializeKey(key, sqlCmd)
                );

            ProcessOutputEntity(holder, rs);

            return rs;
        }
        #endregion
        #region InternalReadByRef
        protected override async Task<IResponseHolder<E>> InternalReadByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, E> holder)
        {
            var rs = await SqlCommandTemplateXml<PersistenceResponseHolder<E>>(Connection
                , StoredProcedureReadByRef
                , sqlCmd => DbSerializeKeyReference(reference, sqlCmd)
                );

            ProcessOutputEntity(holder, rs);

            return rs;
        }
        #endregion
        #region InternalUpdate
        protected override async Task<IResponseHolder<E>> InternalUpdate(K key, PersistenceRequestHolder<K, E> holder)
        {
            var rs = await SqlCommandTemplateXml<PersistenceResponseHolder<E>>(Connection
                , StoredProcedureUpdate
                , sqlCmd => DbSerializeEntity(holder.Rq.Entity, sqlCmd)
                );

            ProcessOutputEntity(holder, rs);

            return rs;
        }
        #endregion
        #region InternalDelete
        protected override async Task<IResponseHolder> InternalDelete(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            var rs = await SqlCommandTemplateXml<PersistenceResponseHolder>(Connection
                , StoredProcedureDelete
                , sqlCmd => DbSerializeKey(key, sqlCmd)
                );

            ProcessOutputKey(false, holder, rs);

            return rs;
        }
        #endregion
        #region InternalDeleteByRef
        protected override async Task<IResponseHolder> InternalDeleteByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            var rs = await SqlCommandTemplateXml<PersistenceResponseHolder>(Connection
                , StoredProcedureDeleteByRef
                , sqlCmd => DbSerializeKeyReference(reference, sqlCmd)
                );

            ProcessOutputKey(true, holder, rs);

            return rs;
        }
        #endregion
        #region InternalVersion
        protected override async Task<IResponseHolder> InternalVersion(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            var rs = await SqlCommandTemplateXml<PersistenceResponseHolder>(Connection
                , StoredProcedureVersion
                , sqlCmd => DbSerializeKey(key, sqlCmd)
                );

            ProcessOutputKey(false, holder, rs);

            return rs;
        }

        #endregion
        #region InternalVersionByRef
        protected override async Task<IResponseHolder> InternalVersionByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            var rs = await SqlCommandTemplateXml<PersistenceResponseHolder>(Connection
                , StoredProcedureVersionByRef
                , sqlCmd => DbSerializeKeyReference(holder.Rq.KeyReference, sqlCmd)
                );

            ProcessOutputKey(true, holder, rs);

            return rs;
        }
        #endregion

        #region ProcessOutputEntity(XElement node, PersistenceResponseHolder<E> rs)
        protected virtual void ProcessOutputEntity(PersistenceRequestHolder<K, E> holder, PersistenceResponseHolder<E> rs)
        {
            if (string.IsNullOrEmpty(rs.Content))
                return;


            rs.Entity = mTransform.PersistenceEntitySerializer.Deserializer(rs.Content);

            try
            {
                if (mTransform.Version != null)
                    rs.VersionId = mTransform.Version.EntityVersionAsString(rs.Entity);
            }
            catch (Exception)
            {
                // Unable to retrieve a key for this entity (might be a collection with no key)
            }

        }
        #endregion
        #region ProcessOutputKey(XElement node, PersistenceResponseHolder rs)
        protected virtual void ProcessOutputKey(bool isByReference, PersistenceRequestHolder<K, Tuple<K, string>> holder, PersistenceResponseHolder rs)
        {
            if (string.IsNullOrEmpty(rs.Content))
                return;

            XElement node = XElement.Parse(rs.Content);

            if (mXmlVersionMaker != null)
            {
                var tuple = mXmlVersionMaker(node);
                rs.VersionId = tuple.Item2;
                if (isByReference)
                    holder.Rq.Key = tuple.Item1;
            }
        }
        #endregion


    }
}
