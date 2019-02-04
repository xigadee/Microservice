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
    /// These extension methods simplify the logging of complex data to a consistent framework.
    /// </summary>
    public static partial class DataCollectionExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="secEvent">The security event.</param>
        /// <returns></returns>
        public static async Task SecurityEvent(this IDataCollection collector, SecurityEvent secEvent)
        {
            collector.Write(secEvent, DataCollectionSupport.Security);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collector">The data collector.</param>
        /// <param name="direction">The security event type.</param>
        /// <param name="ex">The exception.</param>
        /// <returns></returns>
        public static async Task SecurityEvent(this IDataCollection collector, SecurityEventDirection direction, Exception ex)
        {
            collector.Write(new SecurityEvent() { Direction = direction, Ex = ex }, DataCollectionSupport.Security);
        }
    }

    /// <summary>
    /// This is the security event direction.
    /// </summary>
    public enum SecurityEventDirection
    {
        /// <summary>
        /// The direction is not set.
        /// </summary>
        NotSet,
        /// <summary>
        /// The event is an incoming verification.
        /// </summary>
        Verification,
        /// <summary>
        /// The event is an outgoing payload signing event.
        /// </summary>
        Signing
    }
}
