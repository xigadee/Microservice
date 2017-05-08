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
        /// <summary>
        /// This is teh shortcut key to set the remote Tls connection string.
        /// </summary>
        [ConfigSettingKey("TcpTls")]
        public const string KeyTcpTlsConnection = "TcpTlsConnection";

        [ConfigSetting("TcpTls")]
        public static string TcpTlsConnection(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyTcpTlsConnection);

    }
}
