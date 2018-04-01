namespace Xigadee
{
    /// <summary>
    /// Repository setting metadata which is passed to the back end fabric
    /// </summary>
    public class RepositorySettings: RequestSettings
    {
        /// <summary>
        /// This method informs the server to turn off optimistic locking when set to false.
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
        /// This is the optional batch id attached to the document,
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
