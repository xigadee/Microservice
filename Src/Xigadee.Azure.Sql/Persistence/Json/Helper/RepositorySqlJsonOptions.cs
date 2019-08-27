using System.Collections.Generic;

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
        /// <param name="supportExtensions">Specifies whether SQL table extensions are supported.</param>
        /// <param name="prependDllVersion">Specifies whether the DLL version should be prepended to the SQL scripts.</param>
        public RepositorySqlJsonOptions(bool supportExtensions = false, bool prependDllVersion = true)
        {
            SupportsExtension = supportExtensions ? new Option(true, "extension") : new Option(false, "extension");
            SupportsTables = new Option(true, "tables");
            PrependDllVersion = prependDllVersion;
        }

        /// <summary>
        /// Specifies whether the 
        /// </summary>
        public Option SupportsExtension { get; set; }

        /// <summary>
        /// Specifies that the SQL should support the table generation region.
        /// </summary>
        public Option SupportsTables { get; set; }

        /// <summary>
        /// This base option specifies whether the script version should be prepended.
        /// </summary>
        public bool PrependDllVersion { get; set; }

        /// <summary>
        /// Specifies that delete is not supported and should return a 405 error.
        /// The default value is false.
        /// </summary>
        public bool DeleteAs405 { get; set; }

        /// <summary>
        /// This is a list of supported options.
        /// </summary>
        /// <returns>A enumerable list.</returns>
        public IEnumerable<Option> Options()
        {
            yield return SupportsExtension;
            yield return SupportsTables;
        }

        /// <summary>
        /// This method scans the SQL and removes and unsupported SQL options.
        /// </summary>
        /// <param name="sql">The incoming SQL.</param>
        /// <returns>The modified SQL.</returns>
        public string Apply(string sql)
        {
            foreach (var option in Options())
                sql = option.Modify(sql);

            return sql;
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
            /// This is the default constructor.
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

            /// <summary>
            /// The start region definition.
            /// </summary>
            public string RegionStart => $"--#region.{RegionName?.ToLowerInvariant().Trim()}";

            /// <summary>
            /// The endregion definition.
            /// </summary>
            public string RegionEnd => "--#endregion";

            /// <summary>
            /// This method extracts the unsupported sections in the SQL code.
            /// </summary>
            /// <param name="incoming">The incoming SQL.</param>
            /// <returns>The SQL with any unsupported sections removed.</returns>
            public string Modify(string incoming)
            {
                if (!Supported)
                {
                    bool found = true;
                    while (found)
                        incoming = TrimOut(incoming, out found);
                }

                return incoming;
            }

            /// <summary>
            /// This code trims out the unsupported section in the SQL code.
            /// </summary>
            /// <param name="incoming">The incoming SQL code.</param>
            /// <param name="found">output property indicating whether the code was trimmed.</param>
            /// <returns>Returns the parsed SQL code.</returns>
            protected string TrimOut(string incoming, out bool found)
            {
                found = false;

                int postionStart = incoming.IndexOf(RegionStart);

                if (postionStart < 0)
                    return incoming;

                found = true;

                int postionEnd = incoming.IndexOf(RegionEnd, postionStart);

                var start = incoming.Substring(0, postionStart);
                var end = incoming.Substring(postionEnd + RegionEnd.Length);

                return start + $"\r\n-- *** {RegionName} code removed ***\r\n" + end;

            }
        }
    }
}
