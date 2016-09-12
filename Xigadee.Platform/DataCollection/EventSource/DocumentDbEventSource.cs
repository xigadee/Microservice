#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion


using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
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
