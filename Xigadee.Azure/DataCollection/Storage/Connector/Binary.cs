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
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace Xigadee
{
    /// <summary>
    /// This is the abstract class shared by binary output type, i.e. Blob, Queue, File.
    /// </summary>
    /// <typeparam name="O">The request options that determines the retry policy.</typeparam>
    /// <typeparam name="C">The container type.</typeparam>
    public abstract class AzureStorageConnectorBinary<O, C>: AzureStorageConnectorBase<O, C, byte[]>
        where O : Microsoft.WindowsAzure.Storage.IRequestOptions
        where C : AzureStorageContainerBase
    {
        public AzureStorageConnectorBinary()
        {
            Serializer = AzureStorageHelper.DefaultJsonBinarySerializer;
        }       
    }
}
