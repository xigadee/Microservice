//#region using
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Newtonsoft.Json.Linq;
//#endregion
//namespace Xigadee
//{
//    /// <summary>
//    /// This class supports provides generic logging support for Azure Blob Storage.
//    /// </summary>
//    public abstract class AzureStorageLoggingBase<E>: ServiceBase<LoggingStatistics>
//    {
//        #region Declarations
//        /// <summary>
//        /// This is the azure storage wrapper.
//        /// </summary>
//        protected DirectoryInfo mStorage;

//        protected Func<E, string> mIdMaker;
//        protected Func<E, string> mDirectoryMaker;

//        protected string mServiceName;

//        #endregion
//        #region Constructor
//        /// <summary>
//        /// This is the default constructor.
//        /// </summary>
//        /// <param name="credentials">The azure storage credentials.</param>
//        /// <param name="entityName">The options entity name. If this is not presented then the entity name will be used.</param>
//        /// <param name="versionPolicy">The versioning policy.</param>
//        /// <param name="defaultTimeout">The default timeout for async requests.</param>
//        /// <param name="accessType">The azure access type. BlobContainerPublicAccessType.Off is the default.</param>
//        /// <param name="options">The optional blob request options.</param>
//        /// <param name="context">The optional operation context.</param>
//        public AzureStorageLoggingBase(DirectoryInfo credentials
//            , string containerName
//            , string serviceName
//            , Func<E, string> idMaker = null
//            , Func<E, string> directoryMaker = null
//            , TimeSpan? defaultTimeout = null
//            )
//        {
//            mStorage = credentials;//new StorageServiceBase(credentials, containerName, accessType, options, context, defaultTimeout: defaultTimeout);
//            mIdMaker = idMaker ?? IdMaker;
//            mDirectoryMaker = directoryMaker ?? DirectoryMaker;
//            mServiceName = serviceName;
//        }
//        #endregion

//        protected abstract string IdMaker(E data);

//        protected abstract string DirectoryMaker(E data);

//        /// <summary>
//        /// This method serializes the event source in to the specifed binary format.
//        /// </summary>
//        /// <param name="entity">The entity to serialize.</param>
//        /// <returns>Returns a byte array.</returns>
//        protected virtual byte[] Serialize(E entity, out string contentType, out string contentEncoding)
//        {
//            contentType = "application/json; charset=utf-8";
//            contentEncoding = null;
//            var jObj = JObject.FromObject(entity);
//            var body = jObj.ToString();
//            return Encoding.UTF8.GetBytes(body);
//        }

//        protected async Task Output(string id, string directory, E entity)
//        {
//            int start = mStatistics.ActiveIncrement();
//            try
//            {
//                string contentType, contentEncoding;
//                var blob = Serialize(entity, out contentType, out contentEncoding);

//                //await mStorage.CreateOrUpdate(id, blob
//                //    , directory: directory
//                //    , contentType: contentType
//                //    , contentEncoding: contentEncoding
//                //    , createSnapshot: true);
//            }
//            catch (Exception ex)
//            {
//                //We do not throw exceptions here.
//                mStatistics.ErrorIncrement();
//                throw ex;
//            }
//            finally
//            {
//                mStatistics.ActiveDecrement(start);
//            }
//        }

//        protected async Task Output(string id, string directory, byte[] blob, string contentType, string contentEncoding = null)
//        {
//            int start = mStatistics.ActiveIncrement();
//            try
//            {
//                //await mStorage.CreateOrUpdate(id, blob
//                //    , directory: directory
//                //    , contentType: contentType
//                //    , contentEncoding: contentEncoding
//                //    , createSnapshot: true);
//            }
//            catch (Exception ex)
//            {
//                //We do not throw exceptions here.
//                mStatistics.ErrorIncrement();
//                throw ex;
//            }
//            finally
//            {
//                mStatistics.ActiveDecrement(start);
//            }
//        }

//    }
//}
