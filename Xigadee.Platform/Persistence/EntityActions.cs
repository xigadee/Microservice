#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class is a set of shared definitions for the persistence methods.
    /// </summary>
    public static class EntityActions
    {
        public const string Create = "Create";
        public const string Read = "Read";
        public const string ReadByRef = "ReadByRef";
        public const string Update = "Update";
        public const string Delete = "Delete";
        public const string DeleteByRef = "DeleteByRef";
        public const string Version = "Version";
        public const string VersionByRef = "VersionByRef";

        public const string Search = "Search";
        public const string History = "History";
    }
}
