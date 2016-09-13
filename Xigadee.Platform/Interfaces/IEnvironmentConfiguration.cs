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

namespace Xigadee
{
    /// <summary>
    /// This inteface is used by extension method that provide shortcuts for specific key/value pairs
    /// from the environment configuration.
    /// </summary>
    public interface IEnvironmentConfiguration
    {
        Func<string, string, string> Resolver { get; set; }

        bool ResolverFirst { get; set; }

        string PlatformOrConfigCache(string key, string defaultValue = null);

        bool PlatformOrConfigCacheBool(string key, string defaultValue = null);

        int PlatformOrConfigCacheInt(string key, int? defaultValue = default(int?));
    }
}