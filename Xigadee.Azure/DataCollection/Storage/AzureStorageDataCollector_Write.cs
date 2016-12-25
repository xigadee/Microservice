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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json.Linq;

namespace Xigadee
{
    public partial class AzureStorageDataCollector
    {
        #region Write(AzureStorageDataCollectorOptions option, EventBase e)
        /// <summary>
        /// Output the data for the three option types.
        /// </summary>
        /// <param name="option">The storage options</param>
        /// <param name="e">The event object.</param>
        protected void Write(AzureStorageDataCollectorOptions option, EventBase e)
        {
            List<Task> mActions = new List<Task>();

            if ((option.Behaviour & AzureStorageBehaviour.Blob) > 0)
                mActions.Add(WriteBlob(option, e));
            if ((option.Behaviour & AzureStorageBehaviour.Table) > 0)
                mActions.Add(WriteTable(option, e));
            if ((option.Behaviour & AzureStorageBehaviour.Queue) > 0)
                mActions.Add(WriteQueue(option, e));

            Task.WhenAll(mActions).Wait();
        } 
        #endregion

        /// <summary>
        /// This method writes the EventBase entity to blob storage.
        /// </summary>
        /// <param name="option">The options.</param>
        /// <param name="e">The entity.</param>
        /// <returns>An async process.</returns>
        protected async Task WriteBlob(AzureStorageDataCollectorOptions option, EventBase e)
        {
            AzureStorageContainerBlob cont = (option.BlobConverter ?? DefaultBlobConverter)(OriginatorId, e);
            int start = StatisticsInternal.ActiveIncrement(option.Support);

            Guid? traceId = option.ShouldProfile ? (ProfileStart($"{cont.Directory}/{cont.Id}")) : default(Guid?);

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
                if (traceId.HasValue)
                    ProfileEnd(traceId.Value, start, result);
            }
        }
        /// <summary>
        /// This method transforms the EventBase entity to a Table Storage entity and saves it to the table specified in the options.
        /// </summary>
        /// <param name="option">The options.</param>
        /// <param name="e">The entity.</param>
        /// <returns>An async process.</returns>
        protected async Task WriteTable(AzureStorageDataCollectorOptions option, EventBase e)
        {
            AzureStorageContainerTable cont = (option.TableConverter ?? DefaultTableConverter)(OriginatorId, e);
            int start = StatisticsInternal.ActiveIncrement(option.Support);

            Guid? traceId = option.ShouldProfile ? ProfileStart($"{cont.Id}") : default(Guid?);
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
                if (traceId.HasValue)
                    ProfileEnd(traceId.Value, start, result);
            }
        }
        /// <summary>
        /// This method writes the EventBase entity to a blob queue.
        /// </summary>
        /// <param name="option">The options.</param>
        /// <param name="e">The entity.</param>
        /// <returns>An async process.</returns>
        protected async Task WriteQueue(AzureStorageDataCollectorOptions option, EventBase e)
        {
            //CloudQueueMessage message;

            AzureStorageContainerTable cont = (option.TableConverter ?? DefaultTableConverter)(OriginatorId, e);
            int start = StatisticsInternal.ActiveIncrement(option.Support);

            Guid? traceId = option.ShouldProfile ? ProfileStart($"{cont.Id}") : default(Guid?);
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
                if (traceId.HasValue)
                    ProfileEnd(traceId.Value, start, result);
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
