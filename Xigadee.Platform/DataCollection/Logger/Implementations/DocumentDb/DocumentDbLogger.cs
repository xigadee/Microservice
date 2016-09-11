#region using

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is a basic logger based on documentdb
    /// </summary>
    public class DocumentDbLogger : ServiceBase<LoggingStatistics>, ILogger, IServiceOriginator
    {
        #region Declarations
        /// <summary>
        /// This holds the documentDb collection.
        /// </summary>
        protected CollectionHolder mDocDb;
        /// <summary>
        /// This is the logger instance string.
        /// </summary>
        private string mInstance;

        private string mOriginatorId = null;

        private HashSet<LoggingLevel> mLoggingPermitted = null;

        #endregion
        #region Constructor
        /// <summary>
        /// This is the document db persistence agent.
        /// </summary>
        /// <param name="account">This is the documentdb id</param>
        /// <param name="base64key">This is the base64 encoded access key</param>
        /// <param name="databaseId">The is the databaseId name. If the Db does not exist it will be created.</param>
        /// <param name="collectionName">The is the collection name. If the collection does it exist it will be created.</param>
        public DocumentDbLogger(
            string account, string base64key, string database, string databaseCollection = null, LoggingLevel[] logLevels = null)
        {
            mDocDb = new CollectionHolder(account, base64key, database, databaseCollection ?? typeof(LogEvent).Name);
            mInstance = string.Format("Logger_{0}_{1}", Environment.MachineName, DateTime.UtcNow.ToString("yyyyMMddHHmm"));

            if (logLevels!= null)
                mLoggingPermitted  = new HashSet<LoggingLevel>(logLevels.Distinct());
        } 
        #endregion

        #region Log(LogEvent logEvent)
        /// <summary>
        /// This method logs the event to DocumentDb
        /// </summary>
        /// <param name="logEvent">The event to store in the log.</param>
        public async Task Log(LogEvent logEvent)
        {
            int start = StatisticsInternal.ActiveIncrement();
            try
            {

                if (logEvent == null)
                    return;

                if (mLoggingPermitted != null && !mLoggingPermitted.Contains(logEvent.Level))
                    return;

                JObject jObj = JObject.FromObject(logEvent);

                jObj["id"] = mInstance + Guid.NewGuid().ToString("N");

                if (mOriginatorId != null)
                    jObj["$microservice.instance"] = mOriginatorId;

                jObj["$microservice.timestamp"] = DateTime.UtcNow;
                jObj["$microservice.logtype"] = logEvent.GetType().Name;

                await mDocDb.Collection.Create(jObj.ToString());
            }
            catch (Exception ex)
            {
                StatisticsInternal.ErrorIncrement();
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(start);
            }
        } 
        #endregion

        #region OriginatorId
        /// <summary>
        /// This is the originator id of the containing Microservice.
        /// </summary>
        public string OriginatorId
        {
            get
            {
                return mOriginatorId;
            }
            set
            {
                mOriginatorId = value;
                mInstance = string.Format("Logger_{0}", value);
            }
        } 
        #endregion

        protected override void StartInternal()
        {
        }

        protected override void StopInternal()
        {
        }
    }
}
