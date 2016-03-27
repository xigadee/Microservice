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
    /// <summary>
    /// This is the default SQL persistence manager.
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
        /// <param name="entityMaker">The entity maker function.</param>
        /// <param name="versionMaker"></param>
        protected PersistenceManagerHandlerSqlBase(string connection,
            Func<E, K> keyMaker,
            Func<XElement, E> entityMaker,
            Func<XElement, Tuple<K, string>> versionMaker = null,
            PersistenceRetryPolicy retryPolicy = null,
            ResourceProfile resourceProfile = null) 
            : base(connection, keyMaker, entityMaker, versionMaker, retryPolicy, resourceProfile)
        {
        }
        #endregion
    }

    /// <summary>
    /// This is the default SQL persistence manager.
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
        protected Func<XElement, E> mEntityMaker;
        /// <summary>
        /// This function creates the version maker.
        /// </summary>
        protected Func<XElement, Tuple<K, string>> mVersionMaker;
        #endregion
        #region Constructor
        /// <summary>
        /// This is the default constructor with a manual connection string.
        /// </summary>
        /// <param name="connection">The sql datbase connection.</param>
        /// <param name="keyMaker">The key maker function.</param>
        /// <param name="entityMaker">The entity maker function.</param>
        /// <param name="versionMaker"></param>
        protected PersistenceManagerHandlerSqlBase(string connection
            , Func<E, K> keyMaker
            , Func<XElement, E> entityMaker
            , Func<XElement, Tuple<K, string>> versionMaker = null
            , PersistenceRetryPolicy retryPolicy = null
            , ResourceProfile resourceProfile = null
            , ICacheManager<K, E> cacheManager = null) 
            : base(
                  persistenceRetryPolicy: retryPolicy
                  , resourceProfile: resourceProfile
                  , cacheManager: cacheManager
                  , keyMaker: keyMaker)
        {
            Connection = connection;
            mEntityMaker = entityMaker;
            mVersionMaker = versionMaker;
        }
        #endregion

        #region Connection
        /// <summary>
        /// This is the SQL connection.
        /// </summary>
        public string Connection { get; set; }
        #endregion

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
        protected async Task<PersistenceResponseHolder<ET>> SqlCommandTemplate<KT, ET>(
            string DbConnection, 
            string commandname,
            Action<SqlCommand> act,
            Func<XElement, SqlParameter, ET> conv)
        {
            SqlParameter paramReturnValue = null;
            var rs = new PersistenceResponseHolder<ET>();
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

                    ET data = default(ET);
                    try
                    {
                        using (XmlReader xmlR = await sqlCmd.ExecuteXmlReaderAsync())
                        {
                            if (xmlR.Read())
                            {
                                XElement node = null;

                                while (xmlR.ReadState != ReadState.EndOfFile)
                                {
                                    node = XElement.Load(xmlR, LoadOptions.None);
                                }

                                if (node != null)
                                    data = conv(node, paramReturnValue);
                            }
                            else
                                data = conv(null, paramReturnValue);
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        if (paramReturnValue == null 
                            || paramReturnValue.Value == null 
                            || (int)paramReturnValue.Value == 0)
                            throw;

                        data = conv(null, paramReturnValue);
                    }


                    if (paramReturnValue != null && paramReturnValue.Value != null)
                        rs.StatusCode = (int)paramReturnValue.Value;

                    if (rs.StatusCode == 401)
                        Logger.LogMessage(LoggingLevel.Error, string.Format("Sql DB action {0} failed: ", commandname), "SQL");
                    else
                        rs.Entity = data;
                }
            }
            catch (Exception)
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

        #region ProcessCreate

        protected override async Task<IResponseHolder<E>> InternalCreate(PersistenceRequestHolder<K, E> holder)
        {
            return await SqlCommandTemplate(Connection
                , StoredProcedureCreate
                , sqlCmd => DbSerializeEntity(holder.rq.Entity, sqlCmd)
                , (node, sqlParam) => ProcessOutput(node, holder.rs, holder.rq)
                );

        }
        #endregion
        #region ProcessRead
        protected override async Task<IResponseHolder<E>> InternalRead(K key, PersistenceRequestHolder<K, E> holder)
        {
            return await SqlCommandTemplate(Connection
                , StoredProcedureRead
                , sqlCmd => DbSerializeKey(key, sqlCmd)
                , (node, sqlParam) => ProcessOutput(node, holder.rs, holder.rq)
                );

        }
        #endregion
        #region ProcessReadByRef

        protected override async Task<IResponseHolder<E>> InternalReadByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, E> holder)
        {
            return await SqlCommandTemplate(Connection
                , StoredProcedureReadByRef
                , sqlCmd => DbSerializeKeyReference(reference, sqlCmd)
                , (node, sqlParam) => ProcessOutput(node, holder.rs, holder.rq)
                );
        }
        #endregion
        #region ProcessUpdate
        protected override async Task<IResponseHolder<E>> InternalUpdate(PersistenceRequestHolder<K, E> holder)
        {
            return await SqlCommandTemplate(Connection
                , StoredProcedureUpdate
                , sqlCmd => DbSerializeEntity(holder.rq.Entity, sqlCmd)
                , (node, sqlParam) => ProcessOutput(node, holder.rs, holder.rq)
                );
        }
        #endregion
        #region ProcessDelete
        protected override async Task<IResponseHolder> InternalDelete(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            return await SqlCommandTemplate(Connection
                , StoredProcedureDelete
                , sqlCmd => DbSerializeKey(key, sqlCmd)
                , (node, sqlParam) => ProcessOutput(node, holder.rs, holder.rq)
                );
        }
        #endregion
        #region ProcessDeleteByRef
        protected override async Task<IResponseHolder> InternalDeleteByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            return await SqlCommandTemplate(Connection
                , StoredProcedureDeleteByRef
                , sqlCmd => DbSerializeKeyReference(reference, sqlCmd)
                , (node, sqlParam) => ProcessOutput(node, holder.rs, holder.rq)
                );
        }
        #endregion
        #region ProcessVersion
        protected override async Task<IResponseHolder> InternalVersion(K key, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            return await SqlCommandTemplate(Connection
                , StoredProcedureVersion
                , sqlCmd => DbSerializeKey(key, sqlCmd)
                , (node, sqlParam) => ProcessOutput(node, holder.rs, holder.rq)
                );
        }

        #endregion
        #region ProcessVersionByRef

        protected override async Task<IResponseHolder> InternalVersionByRef(Tuple<string, string> reference, PersistenceRequestHolder<K, Tuple<K, string>> holder)
        {
            return await SqlCommandTemplate(Connection, 
                StoredProcedureVersionByRef,
                sqlCmd => DbSerializeKeyReference(holder.rq.KeyReference, sqlCmd),
                (node, sqlParam) => ProcessOutput(node, holder.rs, holder.rq)
                );
        }
        #endregion

        #region ProcessOutput...

        private E ProcessOutput(XElement node, 
            PersistenceRepositoryHolder<K, E> rs, PersistenceRepositoryHolder<K, E> rq)
        {
            if (node == null)
                return default(E);

            var entity = mEntityMaker(node);
            rs.Key = rq.Key;
            rs.KeyReference = rq.KeyReference;

            try
            {
                if (mTransform.KeyMaker != null)
                    rs.Key = mTransform.KeyMaker(entity);

                if (mVersionMaker != null)
                    rs.Settings.VersionId = mVersionMaker(node).Item2;
            }
            catch (Exception)
            {
                // Unable to retrieve a key for this entity (might be a collection with no key)
            }

            rs.KeyReference = new Tuple<string, string>(rs.Key == null ? null : rs.Key.ToString(), rs.Settings == null ? null : rs.Settings.VersionId);

            return entity;
        }

        private Tuple<K, string> ProcessOutput(XElement node, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, PersistenceRepositoryHolder<K, Tuple<K, string>> rq)
        {

            if (node == null)
                return default(Tuple<K, string>);

            rs.Key = rq.Key;
            rs.KeyReference = rq.KeyReference;

            if (mVersionMaker == null)
                return default(Tuple<K, string>);

            var version = mVersionMaker(node);
            if (version == null)
                return default(Tuple<K, string>);

            rs.Key = version.Item1;
            rs.Settings.VersionId = version.Item2;
            rs.KeyReference = new Tuple<string, string>(version.Item1 == null ? null : version.Item1.ToString(), version.Item2);

            return version;
        }

        #endregion
    }
}
