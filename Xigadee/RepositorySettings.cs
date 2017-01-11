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
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xigadee
{
    public class RequestSettings
    {
        /// <summary>
        /// This is the current principal for the thread.
        /// </summary>
        public IPrincipal SecurityPrincipal { get; set; } = Thread.CurrentPrincipal;

        /// <summary>
        /// This is the time the request should wait for a response until signalling a time out.
        /// </summary>
        public TimeSpan? WaitTime { get; set; }

        /// <summary>
        /// http://tools.ietf.org/html/rfc7240
        /// </summary>
        public Dictionary<string, string> Prefer { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> Headers { get; set;  }= new Dictionary<string, string>();

        #region Prefer methods
        protected bool PreferGetBool(string key, string trueValue = "true", bool defaultValue = true)
        {
            if (Prefer == null || !Prefer.ContainsKey(key))
                return defaultValue;

            return Prefer[key].Equals(trueValue, StringComparison.InvariantCultureIgnoreCase);
        }

        protected string PreferGet(string key, string defaultValue = null)
        {
            if (Prefer == null || !Prefer.ContainsKey(key))
                return defaultValue;

            return Prefer[key];
        }

        protected void PreferSet(string key, string value)
        {
            if (Prefer == null)
                Prefer = new Dictionary<string, string>();

            if (!Prefer.ContainsKey(key))
                Prefer.Add(key, value);
            else
                Prefer[key] = value;
        } 
        #endregion
        #region Header methods
        protected string HeadersGet(string key, string defaultValue = null)
        {
            if (Headers == null || !Headers.ContainsKey(key))
                return defaultValue;

            return Headers[key];
        }

        protected void HeadersSet(string key, string value)
        {
            if (Headers == null)
                Headers = new Dictionary<string, string>();

            if (!Headers.ContainsKey(key))
                Headers.Add(key, value);
            else
                Headers[key] = value;
        }
        #endregion

        /// <summary>
        /// Shortcut to retrieve the correlation id
        /// </summary>
        public string CorrelationId
        {
            get
            {
                return HeadersGet("X-CorrelationId");
            }
            set
            {
                HeadersSet("X-CorrelationId", value);
            }
        }

        /// <summary>
        /// This shortcut method is used to inform the server to process the request asynchronously
        /// </summary>
        public bool ProcessAsync
        {
            get
            {
                return PreferGetBool("processasync", defaultValue: false);
            }
            set
            {
                PreferSet("processasync", value ? "true" : "false");
            }
        }
    }

    /// <summary>
    /// Repository setting metadata which is passed to the back end fabric
    /// </summary>
    public class RepositorySettings: RequestSettings
    {

        /// <summary>
        /// This method informs the server to turn off optimistic locking whenset to false.
        /// The default is to be set as true.
        /// </summary>
        public bool OptimisticLocking 
        {
            get
            {
                return PreferGetBool("optimisticlocking", defaultValue: true);
            }
            set
            {
                PreferSet("optimisticlocking", value ? "true" : "false");
            }
        }

        /// <summary>
        /// This is the version id associated with the request.
        /// </summary>
        public string VersionId
        {
            get
            {
                return PreferGet("versionid");
            }
            set
            {
                PreferSet("versionid", value);
            }
        }

        /// <summary>
        /// This is the optional batchid attached to the document,
        /// </summary>
        public string BatchId
        {
            get
            {
                return PreferGet("batchid");
            }
            set
            {
                PreferSet("batchid", value);
            }
        }

        /// <summary>
        /// This is the optional source attached to the document,
        /// </summary>
        public string Source
        {
            get
            {
                return PreferGet("source");
            }
            set
            {
                PreferSet("source", value);
            }
        }

        /// <summary>
        /// This is the optional source id attached to the document,
        /// </summary>
        public string SourceId
        {
            get
            {
                return PreferGet("sourceid");
            }
            set
            {
                PreferSet("sourceid", value);
            }
        }

        /// <summary>
        /// This is the optional source id attached to the document,
        /// </summary>
        public string SourceName
        {
            get
            {
                return PreferGet("sourcename");
            }
            set
            {
                PreferSet("sourcename", value);
            }
        }

        /// <summary>
        /// This shortcut method is used to inform the server to process to use the entity cache if available.
        /// </summary>
        public bool UseCache
        {
            get
            {
                return PreferGetBool("usecache", defaultValue: true);
            }
            set
            {
                PreferSet("usecache", value ? "true" : "false");
            }
        }

    }
}
