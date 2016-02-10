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
    /// This is the default persistence manager.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public abstract class PersistenceManagerHandlerSqlBase<K, E, S> : PersistenceMessageHandlerBase<K, E, S>
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
        protected PersistenceManagerHandlerSqlBase(string connection,
            Func<E, K> keyMaker,
            Func<XElement, E> entityMaker,
            Func<XElement, Tuple<K, string>> versionMaker = null,
            PersistenceRetryPolicy retryPolicy = null, 
            ResourceProfile resourceProfile = null) : base(persistenceRetryPolicy: retryPolicy, resourceProfile: resourceProfile)
        {
            Connection = connection;
            mKeyMaker = keyMaker;
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

        #region CommandTemplate<KT,ET>...
        /// <summary>
        /// This method is used to read an entity from the system.
        /// </summary>
        /// <param Name="id">The entity key.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        protected async Task CommandTemplate<KT, ET>(string DbConnection, string commandname,
            Action<SqlCommand> act,
            Func<SqlCommand, SqlParameter, Task<ET>> execute,
            PersistenceRepositoryHolder<KT, ET> rs)
        {
            SqlParameter paramReturnValue = null;

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

                    ET data = await execute(sqlCmd, paramReturnValue);

                    if (paramReturnValue != null && paramReturnValue.Value != null)
                        rs.ResponseCode = (int)paramReturnValue.Value;

                    if (rs.ResponseCode == 401)
                    {
                        Logger.LogMessage(LoggingLevel.Error, string.Format("Sql DB action {0} failed: ", commandname), "SQL");
                    }
                    else
                        rs.Entity = data;
                }
            }
            catch (Exception)
            {
                if (paramReturnValue != null && paramReturnValue.Value != null)
                    rs.ResponseCode = (int)paramReturnValue.Value;
                else
                    rs.ResponseCode = 500;

                throw;
            }
        }
        #endregion
        #region CommandTemplateSql<R>(string commandname, Action<SqlCommand> act, Func<SqlDataReader,R> conv)
        /// <summary>
        /// This method is used to read an entity from the system.
        /// </summary>
        /// <param Name="id">The entity key.</param>
        /// <returns></returns>
        protected async Task CommandTemplateSql<KT, ET>(string DbConnection, string commandname,
            Action<SqlCommand> act,
            Func<SqlDataReader, ET> conv,
            PersistenceRepositoryHolder<KT, ET> rs)
        {
            await CommandTemplate<KT, ET>(DbConnection, commandname, act,
                async (c, p) =>
                {
                    using (SqlDataReader dr = await c.ExecuteReaderAsync())
                    {
                        if (!dr.HasRows || !dr.Read())
                            return default(ET);

                        return conv(dr);
                    }
                }
                , rs);
        }
        #endregion
        #region CommandTemplateXml<R>(string commandname, Action<SqlCommand> act, Func<SqlCommand,R> conv)
        /// <summary>
        /// This method is used to read an entity from the system.
        /// </summary>
        /// <param Name="id">The entity key.</param>
        /// <returns></returns>
        protected async Task CommandTemplateXml<KT, ET>(string DbConnection, string commandname,
            Action<SqlCommand> act,
            Func<XElement, SqlParameter, ET> conv,
            PersistenceRepositoryHolder<KT, ET> rs)
        {
            await CommandTemplate(DbConnection, commandname, act,
                async (c, p) =>
                {
                    try
                    {
                        using (XmlReader xmlR = await c.ExecuteXmlReaderAsync())
                        {
                            if (xmlR.Read())
                            {
                                XElement node = null;

                                while (xmlR.ReadState != ReadState.EndOfFile)
                                {
                                    node = XElement.Load(xmlR, LoadOptions.None);
                                }

                                if (node != null)
                                    return conv(node, p);
                            }
                            else
                                return conv(null, p);
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        if (p == null || p.Value == null || (int)p.Value == 0)
                            throw;

                        return conv(null, p);
                    }

                    return default(ET);
                }
                , rs);
        }
        #endregion

        #region ProcessCreate
        protected override async Task ProcessCreate(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, 
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            await CommandTemplateXml(Connection, StoredProcedureCreate,
                sqlCmd => DbSerializeEntity(rq.Entity, sqlCmd),
                (node, sqlParam) => ProcessOutput(node, rs, rq), rs);
        }
        #endregion
        #region ProcessRead
        protected override async Task ProcessRead(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, 
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            await CommandTemplateXml(Connection, StoredProcedureRead,
                sqlCmd => DbSerializeKey(rq.Key, sqlCmd),
                (node, sqlParam) => ProcessOutput(node, rs, rq), rs);
        }
        #endregion
        #region ProcessReadByRef
        protected override async Task ProcessReadByRef(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, 
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            await CommandTemplateXml(Connection, StoredProcedureReadByRef,
                sqlCmd => DbSerializeKeyReference(rq.KeyReference, sqlCmd),
                (node, sqlParam) => ProcessOutput(node, rs, rq), rs);
        }
        #endregion
        #region ProcessUpdate
        protected override async Task ProcessUpdate(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs, 
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            await CommandTemplateXml(Connection, StoredProcedureUpdate,
                sqlCmd => DbSerializeEntity(rq.Entity, sqlCmd),
                (node, sqlParam) => ProcessOutput(node, rs, rq), rs);
        }
        #endregion
        #region ProcessDelete
        protected override async Task ProcessDelete(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, 
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            await CommandTemplateXml(Connection, StoredProcedureDelete,
                sqlCmd => DbSerializeKey(rq.Key, sqlCmd),
                (node, sqlParam) => ProcessOutput(node, rs, rq), rs);
        }
        #endregion
        #region ProcessDeleteByRef
        protected override async Task ProcessDeleteByRef(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, 
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            await CommandTemplateXml(Connection, StoredProcedureDeleteByRef,
                sqlCmd => DbSerializeKeyReference(rq.KeyReference, sqlCmd),
                (node, sqlParam) => ProcessOutput(node, rs, rq), rs);
        }
        #endregion
        #region ProcessVersion
        protected override async Task ProcessVersion(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, 
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            await CommandTemplateXml(Connection, StoredProcedureVersion,
                sqlCmd => DbSerializeKey(rq.Key, sqlCmd),
                (node, sqlParam) => ProcessOutput(node, rs, rq), rs);
        }
        #endregion
        #region ProcessVersionByRef
        protected override async Task ProcessVersionByRef(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs, 
            TransmissionPayload prq, List<TransmissionPayload> prs)
        {
            await CommandTemplateXml(Connection, StoredProcedureVersionByRef,
                sqlCmd => DbSerializeKeyReference(rq.KeyReference, sqlCmd),
                (node, sqlParam) => ProcessOutput(node, rs, rq), rs);
        }
        #endregion

        #region Process Sql Output

        private E ProcessOutput(XElement node, PersistenceRepositoryHolder<K, E> rs, PersistenceRepositoryHolder<K, E> rq)
        {
            if (node == null)
                return default(E);

            var entity = mEntityMaker(node);
            rs.Key = rq.Key;
            rs.KeyReference = rq.KeyReference;

            try
            {
                if (mKeyMaker != null)
                    rs.Key = mKeyMaker(entity);

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
