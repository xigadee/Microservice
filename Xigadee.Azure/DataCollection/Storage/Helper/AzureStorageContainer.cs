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

using Microsoft.WindowsAzure.Storage.Table;

namespace Xigadee
{
    /// <summary>
    /// This class represents the location of a blob artefact.
    /// </summary>
    public class AzureStorageContainerBlob: AzureStorageContainerBinaryBase
    {
        /// <summary>
        /// The blob directory.
        /// </summary>
        public string Directory { get; set; }
    }

    /// <summary>
    /// This class represents the location of a queue artefact.
    /// </summary>
    public class AzureStorageContainerQueue: AzureStorageContainerBinaryBase
    {

    }

    /// <summary>
    /// This class represents the location of a blob artefact.
    /// </summary>
    public class AzureStorageContainerTable: AzureStorageContainerBase
    {
        public ITableEntity TableEntity { get; set; }
    }


    /// <summary>
    /// This class represents the location of a blob artefact.
    /// </summary>
    public abstract class AzureStorageContainerBinaryBase: AzureStorageContainerBase
    {
        /// <summary>
        /// This is the binary blob for the entity.
        /// </summary>
        public byte[] Blob { get; set; }
        /// <summary>
        /// This is the content type of the entity.
        /// </summary>
        public string ContentType { get; set; } = "application/json; charset=utf-8";
        /// <summary>
        /// This is the encoding type of the entity.
        /// </summary>
        public string ContentEncoding { get; set; } = null;
    }

    /// <summary>
    /// This class represents the location of a blob artefact.
    /// </summary>
    public abstract class AzureStorageContainerBase
    {
        /// <summary>
        /// The blob identifier.
        /// </summary>
        public string Id { get; set; }
    }
}
