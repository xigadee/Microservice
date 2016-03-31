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
    public abstract class PersistenceManagerHandlerSqlBase<K, E>: PersistenceManagerHandlerSqlBase<K, E, PersistenceStatistics>
        where K : IEquatable<K>
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor with a manual connection string.
        /// </summary>
        /// <param name="connection">The sql datbase connection.</param>
        /// <param name="keyMaker">The key maker function.</param>
        /// <param name="xmlEntityDeserializer">The entity maker function.</param>
        /// <param name="xmlVersionMaker"></param>
        protected PersistenceManagerHandlerSqlBase(string connection
            , Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , Func<XElement, E> xmlEntityDeserializer
            , Func<E, XElement> xmlEntitySerializer
            , Func<XElement, Tuple<K, string>> xmlVersionMaker = null
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<K, string> keySerializer = null
            )
            : base(connection
                  , keyMaker
                  , keyDeserializer
                  , xmlEntityDeserializer
                  , xmlEntitySerializer
                  , xmlVersionMaker: xmlVersionMaker
                  , entityName: entityName
                  , versionPolicy: versionPolicy
                  , defaultTimeout: defaultTimeout
                  , persistenceRetryPolicy: persistenceRetryPolicy
                  , resourceProfile: resourceProfile
                  , cacheManager: cacheManager
                  , referenceMaker: referenceMaker
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
        /// This function is used to create the entity.
        /// </summary>
        protected Func<XElement, E> mXmlEntityDeserializer;

        protected Func<E, XElement> mXmlEntitySerializer;
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
        /// <param name="xmlEntityMaker">The entity maker function.</param>
        /// <param name="xmlVersionMaker"></param>
        protected PersistenceManagerHandlerSqlBase(string connection
            , Func<E, K> keyMaker
            , Func<string, K> keyDeserializer
            , Func<XElement, E> xmlEntityDeserializer
            , Func<E, XElement> xmlEntitySerializer
            , Func<XElement, Tuple<K, string>> xmlVersionMaker = null
            , string entityName = null
            , VersionPolicy<E> versionPolicy = null
            , TimeSpan? defaultTimeout = null
            , PersistenceRetryPolicy persistenceRetryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null
            , Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<K, string> keySerializer = null
            ) 
            : base( persistenceRetryPolicy: persistenceRetryPolicy
                  , resourceProfile: resourceProfile
                  , cacheManager: cacheManager
                  , defaultTimeout: defaultTimeout
                  , entityName: entityName
                  , versionPolicy: versionPolicy
                  , keyMaker:keyMaker
                  , keySerializer: keySerializer
                  , keyDeserializer: keyDeserializer
                  , referenceMaker: referenceMaker
                  )
        {
            Connection = connection;
            mXmlEntityDeserializer = xmlEntityDeserializer;
            mXmlEntitySerializer = xmlEntitySerializer;
            mXmlVersionMaker = xmlVersionMaker;
        }
        #endregion

        #region Connection
        /// <summary>
        /// This is the SQL connection.
        /// </summary>
        public string Connection { get; set; }
        #endregion

        protected override EntityTransformHolder<K, E> EntityTransformCreate(string entityName = null, VersionPolicy<E> versionPolicy = null, Func<E, K> keyMaker = null, Func<string, E> entityDeserializer = null, Func<E, string> entitySerializer = null, Func<K, string> keySerializer = null, Func<string, K> keyDeserializer = null, Func<E, IEnumerable<Tuple<string, string>>> referenceMaker = null)
        {
            var transform =  base.EntityTransformCreate(entityName, versionPolicy, keyMaker, entityDeserializer, entitySerializer, keySerializer, keyDeserializer, referenceMaker);

            transform.EntityDeserializer = (s) => mXmlEntityDeserializer(XElement.Parse(s));
            transform.EntitySerializer = (e) => mXmlEntitySerializer(e).ToString();

            return transform;
        }

        #region Abstract methods
        /// <summary>
        /// This method serializes the entity key in to the Sql command.
        /// </summary>
        /// <param Name="key">The key.</param>
        /// <param Name="cmd">The command.</param>
        public abstract void DbSerializeKey(K key, SqlCommand cmd);
        /// <summary>
        /// This method serializes the entity key in to the Sql command.
        /// </summary>
        /// <param Name="key">The key.</param>
        /// <param Name="cmd">The command.</param>
        public virtual void DbSerializeKeyReference(Tuple<string, string> key, SqlCommand cmd)
        {
            cmd.Parameters.Add("RefType", SqlDbType.NVarChar, 50).Value = key.Item1;
            cmd.Parameters.Add("RefValue", SqlDbType.NVarChar, 255).Value = key.Item2;
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
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected async Task<R> SqlCommandTemplateXml<R>(
            string DbConnection, 
            string commandname,
            Action<SqlCommand> act,
            Action<string, R> conv)
            where R : PersistenceResponseHolder, new()
        {
            SqlParameter paramReturnValue = null;
            R rs = new R();
            try
            {
                using (SqlConnection cn = new SqlConnection(DbConnection))
                {
                    cn.Open();
                    SqlCommand sqlCmd = new SqlCommand(commandname);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Connection = cn;

                    act(sqlCmd);

                    paramReturnValue = new SqlParameter();
                    paramReturnValue.ParameterName = "@return_value";
                    paramReturnValue.SqlDbType = SqlDbType.Int;
                    paramReturnValue.Direction = ParameterDirection.ReturnValue;

                    sqlCmd.Parameters.Add(paramReturnValue);

                    string xmlData = null;

                    try
                    {
                        using (XmlReader xmlR = await sqlCmd.ExecuteXmlReaderAsync())
                        {
                            StringBuilder sb = new StringBuilder();

                            if (xmlR != null)
                            {
                                while (xmlR.ReadState != ReadState.EndOfFile && xmlR.Read())
                                {
                                    sb.AppendLine(xmlR.ReadOuterXml());
                                }
                            }

                            xmlData = sb.ToString();
                        }
                    }
                    catch (InvalidOperationException ioex)
                    {
                        if (paramReturnValue == null 
                            || paramReturnValue.Value == null 
                            || (int)paramReturnValue.Value == 0)
                            throw;
                    }


                    if (paramReturnValue != null && paramReturnValue.Value != null)
                    {
                        rs.StatusCode = (int)paramReturnValue.Value;
                        conv(xmlData, rs);
                    }

                    if (rs.StatusCode == 401)
                        Logger.LogMessage(LoggingLevel.Error, string.Format("Sql DB action {0} failed: ", commandname), "SQL");
                }
            }
            catch (Exception ex)
            {
                if (paramReturnValue != null && paramReturnValue.Value != null)
                    rs.StatusCode = (int)paramReturnValue.Value;
                else
                    rs.StatusCode = 500;

                throw;
            }

            return rs;
        }
        #endregion

        #region InternalCreate

        protected override async Task<IResponseHolder<E>> InternalCreate(PersistenceRequestHolder<K, E> holder)
        {
            return await SqlCommandTemplateXml<PersistenceResponseHolder<E>>(Connection, StoredProcedureCreate
                , sqlCmd => DbSerializeEntity(holder.rq.Entity, sqlCmd)
                , (node, rs) => ProcessOutputEntity(node, rs)
                );

        }
        #endregion
        #region InternalRead
        protected override async Task<IResponseHolder<E>> InternalRead(K key, PersistenceRequestHolder<K, E> holder)
        {
            return await SqlCommandTemplateXml<PersistenceResponseHolder<E>>(Connection, StoredProcedureRead
                , sqlCmd => DbSerializeKey(key, sqlCmd)
                , (node, rs) => ProcessOutputEntity(node, rs)
                );
        }
        #endregion
        #region InternalReadByRef

        protected override async Task<IResponseHolder<E>> InternalReadByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, E> holder)
        {
            return await SqlCommandTemplateXml<PersistenceResponseHolder<E>>(Connection, StoredProcedureReadByRef
                , sqlCmd => DbSerializeKeyReference(reference, sqlCmd)
                , (node, rs) => ProcessOutputEntity(node, rs)
                );
        }
        #endregion
        #region InternalUpdate
        protected override async Task<IResponseHolder<E>> InternalUpdate(PersistenceRequestHolder<K, E> holder)
        {
            return await SqlCommandTemplateXml<PersistenceResponseHolder<E>>(Connection, StoredProcedureUpdate
                , sqlCmd => DbSerializeEntity(holder.rq.Entity, sqlCmd)
                , (node, rs) => ProcessOutputEntity(node, rs)
                );
        }
        #endregion
        #region InternalDelete
        protected override async Task<IResponseHolder> InternalDelete(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            return await SqlCommandTemplateXml<PersistenceResponseHolder>(Connection, StoredProcedureDelete
                , sqlCmd => DbSerializeKey(key, sqlCmd)
                , (node, rs) => ProcessOutputKey(node, rs)
                );
        }
        #endregion
        #region InternalDeleteByRef
        protected override async Task<IResponseHolder> InternalDeleteByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            return await SqlCommandTemplateXml<PersistenceResponseHolder>(Connection, StoredProcedureDeleteByRef
                , sqlCmd => DbSerializeKeyReference(reference, sqlCmd)
                , (node, rs) => ProcessOutputKey(node, rs)
                );
        }
        #endregion
        #region InternalVersion
        protected override async Task<IResponseHolder> InternalVersion(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            return await SqlCommandTemplateXml<PersistenceResponseHolder>(Connection, StoredProcedureVersion
                , sqlCmd => DbSerializeKey(key, sqlCmd)
                , (node, rs) => ProcessOutputKey(node, rs)
                );
        }

        #endregion
        #region InternalVersionByRef
        protected override async Task<IResponseHolder> InternalVersionByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            return await SqlCommandTemplateXml<PersistenceResponseHolder>(Connection, StoredProcedureVersionByRef
                , sqlCmd => DbSerializeKeyReference(holder.rq.KeyReference, sqlCmd)
                , (node, rs) => ProcessOutputKey(node, rs)
                );
        }
        #endregion

        #region ProcessOutputEntity(XElement node, PersistenceResponseHolder<E> rs)
        protected virtual void ProcessOutputEntity(string data, PersistenceResponseHolder<E> rs)
        {
            if (string.IsNullOrEmpty(data))
                return; 


            rs.Entity = mTransform.EntityDeserializer(data);
            rs.Content = data;

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
        protected virtual void ProcessOutputKey(string data, PersistenceResponseHolder rs)
        {
            if (string.IsNullOrEmpty(data))
                return;

            XElement node = XElement.Parse(data);

            if (mXmlVersionMaker != null)
                rs.VersionId = mXmlVersionMaker(node).Item2;
        }
        #endregion


    }
}
