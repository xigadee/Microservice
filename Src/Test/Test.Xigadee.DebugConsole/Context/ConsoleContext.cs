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
using System.Linq;
using System.Collections.Generic;
using Xigadee;
namespace Test.Xigadee
{
    /// <summary>
    /// This class is used to manage the state of the console application.
    /// </summary>
    public class ConsoleContext
    {
        public const string cnShortcut = "console.shortcut";
        public const string cnPersistence = "console.persistence";
        public const string cnApiUri = "console.apiuri";
        public const string cnSlotCount = "console.slotcount";
        public const string cnPersistenceCache = "console.persistencecache";
        public const string cnDefaultUri = "http://localhost:29001";

        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="args">The console arguments.</param>
        public ConsoleContext(string[] args)
        {
            Switches = args?.CommandArgsParse(include:(k,v) => k.StartsWith("console", StringComparison.InvariantCultureIgnoreCase)) ?? new Dictionary<string, string>();
            Config = args?.CommandArgsParse(include: (k, v) => !k.StartsWith("console", StringComparison.InvariantCultureIgnoreCase)) ?? new Dictionary<string, string>();

            if (Switches.ContainsKey(cnPersistence))
                SetServicePersistenceOption(Switches[cnPersistence]);

            if (Switches.ContainsKey(cnPersistenceCache))
                SetServicePersistenceCacheOption(Switches[cnPersistenceCache]);

            Uri api;
            if (Switches.ContainsKey(cnApiUri) && Uri.TryCreate(Switches[cnApiUri], UriKind.Absolute, out api))
                ApiUri = api;
            else
                ApiUri = new Uri(cnDefaultUri);
        }

        /// <summary>
        /// This is the shortcut setting passed in the console switches.
        /// </summary>
        public string Shortcut { get { return Switches.ContainsKey(cnShortcut) ? Switches[cnShortcut] : null; } }

        /// <summary>
        /// This is a list of the console setting switches for the application.
        /// </summary>
        public Dictionary<string, string> Switches { get; protected set; }

        /// <summary>
        /// This is a list of the console setting switches for the application.
        /// </summary>
        public Dictionary<string, string> Config { get; protected set; }

        /// <summary>
        /// This is the listening Uri for the API. It is also used for the APIClient connection.
        /// </summary>
        public Uri ApiUri { get; } 
        /// <summary>
        /// This is the number of slots used in the Microservice.
        /// </summary>
        public int SlotCount => Switches.ContainsKey(cnSlotCount) ?int.Parse(Switches[cnSlotCount]) : Environment.ProcessorCount * 4 * 4;
        /// <summary>
        /// This is the persistence type used in the Server.
        /// </summary>
        public PersistenceOptions PersistenceType { get; set; } = PersistenceOptions.Memory;
        /// <summary>
        /// This is the communication type.
        /// </summary>
        public CommunicationOptions CommunicationType { get; set; } = CommunicationOptions.Local;
        /// <summary>
        /// This is the entity state.
        /// </summary>
        public EntityState EntityState { get; } = new EntityState();

        public RedisCacheModeOptions RedisCache => Switches.ContainsKey(cnPersistenceCache) ? SetServicePersistenceCacheOption(Switches[cnPersistenceCache]): RedisCacheModeOptions.Off;

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
                    PersistenceType = PersistenceOptions.Memory;
                    break;
            }
        }

        private static RedisCacheModeOptions SetServicePersistenceCacheOption(string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "server":
                    return RedisCacheModeOptions.Server;
                case "client":
                    return RedisCacheModeOptions.Client;
                case "clientserver":
                    return RedisCacheModeOptions.ClientServer;
            }

            return RedisCacheModeOptions.Off;
        }
    }
}
