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

namespace Xigadee
{
    public static partial class AzureBaseHelper
    {
        /// <summary>
        /// The reserved keyword.
        /// </summary>
        public const string EventHubs = "EventHubs";

        /// <summary>
        /// This is the Event Hub key type value.
        /// </summary>
        [ConfigSettingKey(EventHubs)]
        public const string KeyEventHubsConnection = "EventHubsConnection";
        /// <summary>
        /// This is the Event Hub connection
        /// </summary>
        /// <param name="config">The Microservice configuration.</param>
        /// <returns>Returns the connection string.</returns>
        [ConfigSetting(EventHubs)]
        public static string EventHubsConnection(this IEnvironmentConfiguration config) => config.PlatformOrConfigCache(KeyEventHubsConnection);

    }
}
