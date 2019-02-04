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
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the root logger interface to support async logging support through the framework.
    /// </summary>
    [Obsolete("Use the new data collection interfaces")]
    public interface IXigadeeLogger
    {
        /// <summary>
        /// This method asynchronously logs an event.
        /// </summary>
        /// <param name="logEvent">The event to log.</param>
        /// <returns>This is an async task.</returns>
        Task Log(LogEvent logEvent);
    }
}
