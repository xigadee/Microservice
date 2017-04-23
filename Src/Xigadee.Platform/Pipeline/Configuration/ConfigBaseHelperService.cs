#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static class ConfigBaseHelperService
    {
        [ConfigSettingKey("Service")]
        public const string KeyEnvironment = "Environment";

        [ConfigSettingKey("Service")]
        public const string KeyClient = "Client";

        [ConfigSettingKey("Service")]
        public const string KeyServiceDisabled = "ServiceDisabled";

        [ConfigSetting("Service")]
        public static bool ServiceDisabled(this IEnvironmentConfiguration config) => config.PlatformOrConfigCacheBool(KeyServiceDisabled);

        [ConfigSetting("Service")]
        public static string Client(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyClient);

        [ConfigSetting("Service")]
        public static string Environment(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyEnvironment);

    }

}
