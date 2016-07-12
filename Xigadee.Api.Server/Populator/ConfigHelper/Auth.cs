using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Auth;

namespace Xigadee
{
    public static class ConfigBaseHelperAuth
    {
        public static bool AuthAllowInsecureHttp(this ConfigWebApi config) => config.PlatformOrConfigCacheBool("AuthAllowInsecureHttp", "true");

        public static Sha512SignatureHelper SignatureProvider(this ConfigWebApi config) => new Sha512SignatureHelper(config.PlatformOrConfigCache("AuthSignature"));


        public static string AuthAudiences(this ConfigWebApi config) => config.PlatformOrConfigCache("AuthAudiences");
        //{"Audiences":[{"Name":"ValueRetail.Web.User","ClientId":"e02920f4b3eb4492b26991030265a0f6","Base64Secret":"G7AIbRzz4r0NC3u5RBnYauveRoCwqm5zmBJU6PDMaa4="},{"Name":"APIClient","ClientId":"4e8ce86a9c914dd8a41c09e0709ef297","Base64Secret":"71UOU6ewhSjuJ/sp+O/1qlvw4m66LkmSsaFljtHan4M=", "CustomTokenExpirationMin":"525949"}]}

        public static string AuthIssuer(this ConfigWebApi config) => config.PlatformOrConfigCache("AuthIssuer");
        //https://vrapibffstaging.cloudapp.net/

        public static string AuthTokenLifeInMin(this ConfigWebApi config) => config.PlatformOrConfigCache("AuthTokenLifeInMin", "30");

    }
}
