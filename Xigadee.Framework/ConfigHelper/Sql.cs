using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ConfigHelperSql
    {
        public static string SqlConnection(this ConfigBase config) => config.PlatformOrConfigCache("SqlConnection");

    }
}
