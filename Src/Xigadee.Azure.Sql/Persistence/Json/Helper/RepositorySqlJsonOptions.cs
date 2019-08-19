using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class holds the set of SQL generation options.
    /// </summary>
    public class RepositorySqlJsonOptions
    {
        /// <summary>
        /// This is the constructor for the SQL options.
        /// </summary>
        /// <param name="supportExtensions"></param>
        public RepositorySqlJsonOptions(bool supportExtensions = false)
        {
            SupportsExtension = supportExtensions ? new Option(true, "extension") : new Option();
        }
        /// <summary>
        /// Specifies whether the 
        /// </summary>
        public Option SupportsExtension { get; private set; }

        /// <summary>
        /// This is a list of supported options.
        /// </summary>
        /// <returns>A enumerable list.</returns>
        public IEnumerable<Option> Options()
        {
            yield return SupportsExtension;
        }

        /// <summary>
        /// This is the legacy setting.
        /// </summary>
        public static RepositorySqlJsonOptions Legacy => new RepositorySqlJsonOptions();

        /// <summary>
        /// The option.
        /// </summary>
        public class Option
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="supported">Specifies whether the option is supported.</param>
            /// <param name="regionName">The region name.</param>
            public Option(bool supported = false, string regionName = null)
            {
                Supported = supported;
                RegionName = regionName;
            }
            /// <summary>
            /// The extension is supported.
            /// </summary>
            public bool Supported { get; }
            /// <summary>
            /// The SQL region name.
            /// </summary>
            public string RegionName { get; }
        }
    }
}
