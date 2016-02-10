#region using
using Microsoft.WindowsAzure.Storage.Auth;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Hadoop.Avro;
using Microsoft.Hadoop.Avro.Container;
using Newtonsoft.Json.Linq;
#endregion
namespace Xigadee
{
    public class AvroFailureWrapper
    {
        public string OriginalType { get; set; }

        public string Exception { get; set;}

        public string JsonBody { get; set; }

    }

    public class AzureStorageEventSourceAvro: AzureStorageLoggingBase<EventSourceEntryBase>, IEventSource
    {
        private readonly bool mDailyLog;
        AvroSerializerSettings mSettings;

        public AzureStorageEventSourceAvro(StorageCredentials credentials, string serviceName, string containerName = "datapump", bool dailyLog = false
            , ResourceProfile resourceProfile = null) 
            : base(credentials, containerName, serviceName, resourceProfile:resourceProfile)
        {
            mDailyLog = dailyLog;
            mSettings = new AvroSerializerSettings()
            {
                Resolver = new AvroPublicMemberContractResolver(true),
                GenerateDeserializer = true,
                GenerateSerializer = true
            };
        }

        public async Task Write<K, E>(string originatorId, EventSourceEntry<K, E> entry, DateTime? utcTimeStamp = default(DateTime?), bool sync = false)
        {
            await Output(mIdMaker(entry), mDirectoryMaker(entry), AvroSerialize(entry), "avro/binary");
        }

        protected override string DirectoryMaker(EventSourceEntryBase data)
        {
            string dailyFolder = mDailyLog ? string.Format("{0:yyyy-MM-dd}/", data.UTCTimeStamp) : string.Empty;
            return string.Format("{0}/{1}{2}", mServiceName, dailyFolder, data.EntityType);
        }

        private byte[] AvroSerialize<K, E>(EventSourceEntry<K, E> entry, bool noRecurse = false)
        {
            if (entry != null)
                try
                {
                    using (var outputStream = new MemoryStream())
                    {
                        // Compress the data using the Deflate codec.
                        using (var avroWriter = AvroContainer.CreateWriter<EventSourceEntry<K, E>>(outputStream, mSettings, Codec.Deflate))
                        {
                            using (var seqWriter = new SequentialWriter<EventSourceEntry<K, E>>(avroWriter, 24))
                            {
                                // Serialize the data to stream using the sequential writer.
                                seqWriter.Write(entry);
                                seqWriter.Sync();
                            }

                            outputStream.Position = 0;
                            byte[] blob = new byte[outputStream.Length];
                            outputStream.Read(blob, 0, blob.Length);
                            return blob;
                        }
                    }
                }
                catch (SerializationException ex)
                {
                    string warning = string.Format("Unable to serialize Avro for {0}-{1}-{2}-{3}", entry.EntityType, entry.Key, entry.EntityVersion, ex);
                    Logger.LogMessage(LoggingLevel.Warning, warning, "AvroStorage");

                    if (!noRecurse)
                    {
                        return AvroSerialize(ErrorWrapper(entry, warning), true);
                    }
                }

            return new byte[0];
        }

        private EventSourceEntry<K, AvroFailureWrapper> ErrorWrapper<K,E>(EventSourceEntry<K, E> entry, string warning)
        {
            try
            {
                var failure = new AvroFailureWrapper();
                failure.OriginalType = typeof(E).Name;
                failure.Exception = warning;
                var jObj = JObject.FromObject(entry.Entity);
                failure.JsonBody = jObj.ToString();

                var wrapper = new EventSourceEntry<K, AvroFailureWrapper>((e) => entry.Key);

                wrapper.BatchId = entry.BatchId;
                wrapper.CorrelationId = entry.CorrelationId;
                wrapper.Entity = failure;
                wrapper.EntityKey = entry.EntityKey;
                wrapper.EntityType = "AvroFailureWrapper";
                wrapper.EntityVersion = entry.EntityVersion;
                wrapper.EntityVersionOld = entry.EntityVersionOld;
                wrapper.EventType = entry.EventType;
                wrapper.UTCTimeStamp = entry.UTCTimeStamp;

                return wrapper;
            }
            catch (Exception)
            {

            }

            return null;
        }


        protected override string IdMaker(EventSourceEntryBase data)
        {
            return string.Format("{0}.avro", string.Join("_", data.Key.Split(Path.GetInvalidFileNameChars())));
        }
    }
}
