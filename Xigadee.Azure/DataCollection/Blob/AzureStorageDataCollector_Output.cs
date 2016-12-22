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
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;

namespace Xigadee
{
    public partial class AzureStorageDataCollector
    {
        protected async Task OutputBlob(AzureStorageDataCollectorOptions option, EventBase e)
        {
            AzureStorageContainerBlob cont = (option.BlobConverter ?? DefaultBlobConverter)(OriginatorId, e);
            int start = StatisticsInternal.ActiveIncrement(option.Support);

            Guid traceId = ProfileStart($"{cont.Directory}/{cont.Id}");

            var result = ResourceRequestResult.Unknown;
            try
            {
                await mStorage.CreateOrUpdate(cont.Id
                    , cont.Blob
                    , directory: cont.Directory
                    , contentType: cont.ContentType
                    , contentEncoding: cont.ContentEncoding
                    , createSnapshot: false);

                result = ResourceRequestResult.Success;
            }
            catch (StorageThrottlingException)
            {
                result = ResourceRequestResult.Exception;
                throw;
            }
            catch (Exception ex)
            {
                result = ResourceRequestResult.Exception;
                //Collector?.LogException(string.Format("Unable to output {0} to {1} for {2}", id, directory, typeof(E).Name), ex);
                StatisticsInternal.ErrorIncrement(option.Support);
                throw;
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(option.Support, start);
                ProfileEnd(traceId, start, result);
            }
        }

        protected async Task OutputTable(AzureStorageDataCollectorOptions option, EventBase e)
        {
            AzureStorageContainerTable cont = (option.TableConverter ?? DefaultTableConverter)(OriginatorId, e);
            int start = StatisticsInternal.ActiveIncrement(option.Support);

            Guid traceId = ProfileStart($"{cont.Id}");
            var result = ResourceRequestResult.Unknown;
            try
            {
                //await mStorage.CreateOrUpdate(cont.Id
                //    , cont.Blob
                //    , directory: cont.Directory
                //    , contentType: cont.ContentType
                //    , contentEncoding: cont.ContentEncoding
                //    , createSnapshot: false);

                //result = ResourceRequestResult.Success;
            }
            catch (StorageThrottlingException)
            {
                result = ResourceRequestResult.Exception;
                throw;
            }
            catch (Exception ex)
            {
                result = ResourceRequestResult.Exception;
                //Collector?.LogException(string.Format("Unable to output {0} to {1} for {2}", id, directory, typeof(E).Name), ex);
                StatisticsInternal.ErrorIncrement(option.Support);
                throw;
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(option.Support, start);
                ProfileEnd(traceId, start, result);
            }
        }

        private AzureStorageContainerTable DefaultTableConverter(MicroserviceId id, EventBase e)
        {
            var cont = new AzureStorageContainerTable();

            return cont;
        }

        private AzureStorageContainerBlob DefaultBlobConverter(MicroserviceId id, EventBase e)
        {
            var cont = new AzureStorageContainerBlob();

            var jObj = JObject.FromObject(e);
            var body = jObj.ToString();

            cont.Blob = Encoding.UTF8.GetBytes(body);

            return cont;
        }
    }
}
