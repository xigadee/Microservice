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

namespace Xigadee
{
    public partial class AzureStorageDataCollector: DataCollectorHolder
    {
        private async Task Output(DataCollectionSupport support
            , string id
            , string directory
            , byte[] blob
            , string contentType
            , string contentEncoding
            , bool useEncryption = true)
        {
            int start = StatisticsInternal.ActiveIncrement(support);
            Guid traceId = ProfileStart(id, directory);
            var result = ResourceRequestResult.Unknown;
            try
            {
                await mStorage.CreateOrUpdate(id, blob
                    , directory: directory
                    , contentType: contentType
                    , contentEncoding: contentEncoding
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
                StatisticsInternal.ErrorIncrement(support);
                throw;
            }
            finally
            {
                StatisticsInternal.ActiveDecrement(support,start);
                ProfileEnd(traceId, start, result);
            }
        }

        private Guid ProfileStart(string id, string directory)
        {
            if (mResourceConsumer == null)
                return Guid.NewGuid();

            return mResourceConsumer.Start(string.Format("{0}/{1}", directory, id), Guid.NewGuid());
        }

        private void ProfileEnd(Guid profileId, int start, ResourceRequestResult result)
        {
            if (mResourceConsumer == null)
                return;

            mResourceConsumer.End(profileId, start, result);
        }

        private void ProfileRetry(Guid profileId, int retryStart, ResourceRetryReason reason)
        {
            if (mResourceConsumer == null)
                return;

            mResourceConsumer.Retry(profileId, retryStart, reason);
        }
    }
}
