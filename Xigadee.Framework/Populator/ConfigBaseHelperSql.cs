using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ConfigBaseHelperSql
    {
        public const string KeySqlConnection = "SqlConnection";

        public static string SqlConnection(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeySqlConnection);

    }
}
