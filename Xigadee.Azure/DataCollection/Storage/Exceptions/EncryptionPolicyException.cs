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
    /// This exception is thrown when a storage logger is set to throw an exception
    /// when encryption is set as mandatory, but an encryption handler has not been set.
    /// </summary>
    public class AzureStorageDataCollectorEncryptionPolicyException:Exception
    {
        public AzureStorageDataCollectorEncryptionPolicyException(DataCollectionSupport support)
        {
            Support = support;
        }
        /// <summary>
        /// This is the type of logging.
        /// </summary>
        public DataCollectionSupport Support { get; set; }
    }
}
