using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// Repository setting metadata which is passed to the back end fabric
    /// </summary>
    public class RepositorySettings
    {
        #region Constructor
        /// <summary>
        /// The default constructor
        /// </summary>
        public RepositorySettings()
        {
            Prefer = new Dictionary<string, string>();

            Headers = new Dictionary<string, string>();
        } 
        #endregion

        private bool PreferGetBool(string key, string trueValue = "true", bool defaultValue = true)
        {
            if (Prefer == null || !Prefer.ContainsKey(key))
                return defaultValue;

            return Prefer[key].Equals(trueValue, StringComparison.InvariantCultureIgnoreCase);
        }

        private string PreferGet(string key, string defaultValue = null)
        {
            if (Prefer == null || !Prefer.ContainsKey(key))
                return defaultValue;

            return Prefer[key];
        }

        private void PreferSet(string key, string value)
        {
            if (Prefer == null)
                Prefer = new Dictionary<string, string>();

            if (!Prefer.ContainsKey(key))
                Prefer.Add(key, value);
            else
                Prefer[key] = value;
        }

        private string HeadersGet(string key, string defaultValue = null)
        {
            if (Headers == null || !Headers.ContainsKey(key))
                return defaultValue;

            return Headers[key];
        }

        private void HeadersSet(string key, string value)
        {
            if (Headers == null)
                Headers = new Dictionary<string, string>();

            if (!Headers.ContainsKey(key))
                Headers.Add(key, value);
            else
                Headers[key] = value;
        }

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

        public TimeSpan? WaitTime { get; set; }

        /// <summary>
        /// http://tools.ietf.org/html/rfc7240
        /// </summary>
        public Dictionary<string, string> Prefer { get; set; }
        public Dictionary<string, string> Headers { get; set; }
    }
}
