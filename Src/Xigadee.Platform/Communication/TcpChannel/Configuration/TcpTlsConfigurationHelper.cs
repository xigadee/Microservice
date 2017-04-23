using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the configuration helper.
    /// </summary>
    public static class TcpTlsConfigurationHelper
    {
        [ConfigSettingKey("TcpTls")]
        public const string KeyTcpTlsConnection = "TcpTlsConnection";

        [ConfigSetting("TcpTls")]
        public static string TcpTlsConnection(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyTcpTlsConnection);

    }
}
