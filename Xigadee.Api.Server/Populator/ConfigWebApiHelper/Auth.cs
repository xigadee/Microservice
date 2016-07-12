using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static class ConfigWebApiHelperAuth
    {
        [ConfigSettingKey("WebApiAuth")]
        public const string KeyAuthAllowInsecureHttp = "AuthAllowInsecureHttp";
        [ConfigSettingKey("WebApiAuth")]
        public const string KeyAuthSignature = "AuthSignature";
        [ConfigSettingKey("WebApiAuth")]
        public const string KeyAuthAudiences = "AuthAudiences";
        [ConfigSettingKey("WebApiAuth")]
        public const string KeyAuthIssuer = "AuthIssuer";
        [ConfigSettingKey("WebApiAuth")]
        public const string KeyAuthTokenLifeInMin = "AuthTokenLifeInMin";

        [ConfigSetting("WebApiAuth")]
        public static bool AuthAllowInsecureHttp(this ConfigWebApi config) => config.PlatformOrConfigCacheBool(KeyAuthAllowInsecureHttp, "true");

        [ConfigSetting("WebApiAuth")]
        public static Sha512SignatureHelper SignatureProvider(this ConfigWebApi config) => new Sha512SignatureHelper(config.PlatformOrConfigCache(KeyAuthSignature));

        [ConfigSetting("WebApiAuth")]
        public static string AuthAudiences(this ConfigWebApi config) => config.PlatformOrConfigCache(KeyAuthAudiences);
        //{"Audiences":[{"Name":"ValueRetail.Web.User","ClientId":"e02920f4b3eb4492b26991030265a0f6","Base64Secret":"G7AIbRzz4r0NC3u5RBnYauveRoCwqm5zmBJU6PDMaa4="},{"Name":"APIClient","ClientId":"4e8ce86a9c914dd8a41c09e0709ef297","Base64Secret":"71UOU6ewhSjuJ/sp+O/1qlvw4m66LkmSsaFljtHan4M=", "CustomTokenExpirationMin":"525949"}]}

        [ConfigSetting("WebApiAuth")]
        public static string AuthIssuer(this ConfigWebApi config) => config.PlatformOrConfigCache(KeyAuthIssuer);
        //https://vrapibffstaging.cloudapp.net/

        [ConfigSetting("WebApiAuth")]
        public static string AuthTokenLifeInMin(this ConfigWebApi config) => config.PlatformOrConfigCache(KeyAuthTokenLifeInMin, "30");

    }
}
