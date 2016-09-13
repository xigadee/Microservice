
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This EventSource uses the REST DocumentDb libraries to log entity change.
    /// </summary>
    public class DocumentDbEventSource:IEventSource
    {
        #region Declarations
        protected CollectionHolder mDocDb;

        private string mInstance;

        private class EventSourceWrapper<K,E>
        {
            public string OriginatorId { get; set; }
            public EventSourceEntry<K, E> Entry { get; set; }
            public DateTime? Timestamp { get; set; }
        }

        #endregion
        #region Constructor
        /// <summary>
        /// This is the document db persistence agent.
        /// </summary>
        /// <param name="account">This is the documentdb id</param>
        /// <param name="base64key">This is the base64 encoded access key</param>
        /// <param name="databaseId">The is the databaseId name. If the Db does not exist it will be created.</param>
        /// <param name="collectionName">The is the collection name. If the collection does it exist it will be created.</param>
        public DocumentDbEventSource(
            string account, string base64key, string database, string databaseCollection = null)
        {
            mDocDb = new CollectionHolder(account, base64key, database, databaseCollection ?? typeof(EventSourceEntry).Name);
            mInstance = string.Format("EventSource_{0}_{1}_", Environment.MachineName, DateTime.UtcNow.ToString("MMddHHmm"));
        } 
        #endregion

        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
            var esw = new EventSourceWrapper<K,E>() { OriginatorId = originatorId, Entry = entry, Timestamp = utcTimeStamp ?? DateTime.UtcNow };
            JObject jObj = JObject.FromObject(esw);

            jObj["id"] = mInstance + Guid.NewGuid().ToString("N");

            await mDocDb.Collection.Create(jObj.ToString());
        }

        /// <summary>
        /// This is the event source identifier.
        /// </summary>
        public string Name
        {
            get
            {
                return "DocumentDb";
            }
        }
    }
}
