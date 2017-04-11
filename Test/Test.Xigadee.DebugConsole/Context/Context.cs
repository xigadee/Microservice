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

using System;
using System.Collections.Generic;
using Xigadee;
namespace Test.Xigadee
{

    /// <summary>
    /// This class is used to manage the state of the console application.
    /// </summary>
    class ConsoleContext
    {
        public ConsoleContext(string[] args = null)
        {
            Switches = args?.CommandArgsParse() ?? new Dictionary<string, string>();

            Client = new MicroserviceWrapper(Switches);
            Server = new MicroserviceWrapper(Switches);

            //ApiServer = new PopulatorApiService();

            if (Switches.Count > 0)
            {
                if (Switches.ContainsKey("apiuri"))
                    ApiServer.ApiUri = new Uri(Switches["apiuri"]);
                else
                    ApiServer.ApiUri = new Uri("http://localhost:29001");

                if (Switches.ContainsKey("persistence"))
                    SetServicePersistenceOption(Switches["persistence"]);

                if (Switches.ContainsKey("persistencecache"))
                    SetServicePersistenceCacheOption(Switches["persistencecache"]);

                SlotCount = Switches.ContainsKey("processes") ?
                    int.Parse(Switches["processes"]) : Environment.ProcessorCount * 4 * 4;
            }
        }

        /// <summary>
        /// This is the shortcut setting passed in the console switches.
        /// </summary>
        public string Shortcut { get { return Switches.ContainsKey("shortcut") ? Switches["shortcut"] : null; } }

        /// <summary>
        /// This is a list of the console setting switches for the application.
        /// </summary>
        public Dictionary<string, string> Switches { get; set; }

        /// <summary>
        /// This is the client Microservice.
        /// </summary>
        public MicroserviceWrapper Client { get; private set; }
        /// <summary>
        /// This is the server Microservice.
        /// </summary>
        public MicroserviceWrapper Server { get; private set; }
        /// <summary>
        /// This is the Api instance.
        /// </summary>
        public PopulatorApiService ApiServer { get; private set; }

        public int SlotCount { get; set; }

        public PersistenceOptions PersistenceType { get; set; } = PersistenceOptions.DocumentDb;

        public EntityState EntityState { get; } = new EntityState();

        private void SetServicePersistenceOption(string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "sql":
                    PersistenceType = PersistenceOptions.Sql;
                    break;
                case "blob":
                    PersistenceType = PersistenceOptions.Blob;
                    break;
                case "documentdbsdk":
                    PersistenceType = PersistenceOptions.DocumentDbSdk;
                    break;
                case "documentdb":
                    PersistenceType = PersistenceOptions.DocumentDb;
                    break;
                case "redis":
                    PersistenceType = PersistenceOptions.RedisCache;
                    break;
                case "memory":
                    PersistenceType = PersistenceOptions.Memory;
                    break;
                default:
                    PersistenceType = PersistenceOptions.DocumentDb;
                    break;
            }
        }

        private void SetServicePersistenceCacheOption(string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "server":
                    Server.RedisCacheEnabled = true;
                    break;
                case "client":
                    Client.RedisCacheEnabled = true;
                    break;
                case "clientserver":
                    Server.RedisCacheEnabled = Client.RedisCacheEnabled = true;
                    break;
            }
        }

    }
}
