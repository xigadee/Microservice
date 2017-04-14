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
    [Flags]
    public enum RedisCacheMode
    {
        Off = 0,
        Server = 1,
        Client = 2,
        ClientServer = 3
    }

    /// <summary>
    /// This class is used to manage the state of the console application.
    /// </summary>
    class ConsoleContext
    {
        public ConsoleContext(string[] args = null)
        {
            Switches = args?.CommandArgsParse() ?? new Dictionary<string, string>();

            if (Switches.ContainsKey("persistence"))
                SetServicePersistenceOption(Switches["persistence"]);
        }

        /// <summary>
        /// This is the shortcut setting passed in the console switches.
        /// </summary>
        public string Shortcut { get { return Switches.ContainsKey("shortcut") ? Switches["shortcut"] : null; } }

        /// <summary>
        /// This is a list of the console setting switches for the application.
        /// </summary>
        public Dictionary<string, string> Switches { get; set; }

        public Uri ApiUri => Switches.ContainsKey("apiuri")?new Uri(Switches["apiuri"]):new Uri("http://localhost:29001");

        public int SlotCount => Switches.ContainsKey("processes")?int.Parse(Switches["processes"]) : Environment.ProcessorCount * 4 * 4;

        public PersistenceOptions PersistenceType { get; set; } = PersistenceOptions.DocumentDb;

        public EntityState EntityState { get; } = new EntityState();

        public RedisCacheMode RedisCache => Switches.ContainsKey("persistencecache") ? SetServicePersistenceCacheOption(Switches["persistencecache"]): RedisCacheMode.Off;

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

        private static RedisCacheMode SetServicePersistenceCacheOption(string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "server":
                    return RedisCacheMode.Server;
                case "client":
                    return RedisCacheMode.Client;
                case "clientserver":
                    return RedisCacheMode.ClientServer;
            }

            return RedisCacheMode.Off;

        }

    }
}
