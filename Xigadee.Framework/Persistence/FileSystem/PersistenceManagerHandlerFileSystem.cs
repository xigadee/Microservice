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

//#region using

//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using System.IO;
//using Xigadee.Persistence;
//#endregion
//namespace Xigadee
//{
//    /// <summary>
//    /// This is the persistence manager based on local file storage. 
//    /// It is mainly used for testing. The local file storage uses Transactional NTFS and so 
//    /// requires call to theunderlying Kernel.
//    /// </summary>
//    /// <typeparam name="K">The key type.</typeparam>
//    /// <typeparam name="E">The entity type.</typeparam>
//    public class PersistenceManagerHandlerFileSystem<K, E>: PersistenceManagerHandlerJsonBase<K, E>
//        where K : IEquatable<K>
//    {
//        #region Declarations
//        /// <summary>
//        /// This is the folder where entities should be stored.
//        /// </summary>
//        protected DirectoryInfo mStorage;
//        /// <summary>
//        /// This is the function that creates Ids for the storage container.
//        /// </summary>
//        protected Func<K, string> mIdMaker;
//        #endregion
//        #region Constructor

//        /// <summary>
//        /// This is the default constructor. The root and entity folder will be created when this
//        /// constructor is called.
//        /// </summary>
//        /// <param name="idMaker"></param>
//        /// <param name="entityName">The options entity name. If this is not presented then the entity name will be used.</param>
//        /// <param name="versionPolicy">The versioning policy.</param>
//        /// <param name="defaultTimeout">The default timeout for async requests.</param>
//        /// <param name="rootFolder"></param>
//        /// <param name="keyMaker"></param>
//        /// <param name="retryPolicy"></param>
//        public PersistenceManagerHandlerFileSystem(DirectoryInfo rootFolder
//            , Func<E, K> keyMaker
//            , Func<K, string> idMaker
//            , string entityName = null
//            , VersionPolicy<E> versionPolicy = null
//            , TimeSpan? defaultTimeout = null
//            , PersistenceRetryPolicy persistenceRetryPolicy = null
//            , ResourceProfile resourceProfile = null
//            )
//            : base(entityName, versionPolicy, defaultTimeout, persistenceRetryPolicy:persistenceRetryPolicy, resourceProfile:resourceProfile)
//        {
//            if (!rootFolder.Exists)
//                rootFolder.Create();

//            var folder = Path.Combine(rootFolder.FullName, entityName ?? typeof(E).Name);
//            mStorage = new DirectoryInfo(folder);
//            if (mStorage.Exists)
//                mStorage.Create();

//            mKeyMaker = keyMaker;
//            mIdMaker = idMaker;
//        }
//        #endregion

//        #region KeyStringMaker(K key)
//        /// <summary>
//        /// This method used the function passed in the constructor to convert the key to a string
//        /// that can be used as reference key for the underlying file storage technology.
//        /// </summary>
//        /// <param name="key">The entity key.</param>
//        /// <returns>Returns a string representation.</returns>
//        protected override string KeyStringMaker(K key)
//        {
//            return mIdMaker(key);
//        }
//        #endregion

//        #region StartInternal()
//        /// <summary>
//        /// This override checks whether the operating system supports Transactional NTFS.
//        /// </summary>
//        protected override void StartInternal()
//        {
//            if (!TxFTransaction.IsSupported)
//                throw new NotSupportedException("PersistenceManagerHandlerFileSystem requires the operating systems to support Transactional NTFS.");

//            base.StartInternal();
//        }
//        #endregion

//        #region ProcessCreate
//        /// <summary>
//        /// Create
//        /// </summary>
//        /// <param name="rq">The request.</param>
//        /// <param name="rs">The response.</param>
//        /// <param name="prq">The incoming payload.</param>
//        /// <param name="prs">The outgoing payload.</param>
//        protected override async Task ProcessCreate(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
//            TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            var jsonHolder = JsonMaker(rq);
//            var blob = Encoding.UTF8.GetBytes(jsonHolder.Json);
//            var result = new FileResponseHolder { StatusCode = 408 };

//            var id = KeyStringMaker(jsonHolder.Key);
//            var path = Path.Combine(mStorage.FullName , id);

//            using (var transaction = new TxFTransaction(false))
//            {
//                try
//                {
//                    var actual = TxFFile.CreateFile(path, TxFFile.CreationDisposition.CreatesNewfileAlways, transaction);
//                    int ioResult = TxFFile.WriteFile(actual, blob);
//                    transaction.Commit();
//                    result = new FileResponseHolder { StatusCode = 200, IsSuccess = true };
//                }
//                catch (TxFException tex)
//                {
//                    transaction.Rollback();
//                    result = new FileResponseHolder { StatusCode = 408, IsSuccess = false };                 
//                }
//            }

//            ProcessOutputEntity(rs, result);
//        }
//        #endregion
//        #region ProcessRead
//        /// <summary>
//        /// Read
//        /// </summary>
//        /// <param name="rq">The request.</param>
//        /// <param name="rs">The response.</param>
//        /// <param name="prq">The incoming payload.</param>
//        /// <param name="prs">The outgoing payload.</param>
//        protected override async Task ProcessRead(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
//            TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            var result = new FileResponseHolder { StatusCode = 408 };
//            //var result = await mStorage.Read(mIdMaker(rq.Key), directory: mDirectory);

//            ProcessOutputEntity(rs, result);
//        }
//        #endregion

//        #region ProcessUpdate
//        /// <summary>
//        /// Update
//        /// </summary>
//        /// <param name="rq">The request.</param>
//        /// <param name="rs">The response.</param>
//        /// <param name="prq">The incoming payload.</param>
//        /// <param name="prs">The outgoing payload.</param>
//        protected override async Task ProcessUpdate(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
//            TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            var jsonHolder = JsonMaker(rq);
//            var blob = Encoding.UTF8.GetBytes(jsonHolder.Json);
//            var result = new FileResponseHolder { StatusCode = 408 };

//            //var result = await mStorage.Update(jsonHolder.Id, blob
//            //    , contentType: "application/json; charset=utf-8"
//            //    , version: jsonHolder.Version, directory: mDirectory);

//            ProcessOutputEntity(rs, result);
//        }
//        #endregion
//        #region ProcessDelete
//        /// <summary>
//        /// Delete request.
//        /// </summary>
//        /// <param name="rq">The request.</param>
//        /// <param name="rs">The response.</param>
//        /// <param name="prq">The incoming payload.</param>
//        /// <param name="prs">The outgoing payload.</param>
//        protected override async Task ProcessDelete(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
//            TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            var result = new FileResponseHolder { StatusCode = 408 };
//            //var result = await mStorage.Delete(mIdMaker(rq.Key), directory: mDirectory);

//            ProcessOutputKey(rq, rs, result);
//        }
//        #endregion
//        #region ProcessVersion
//        /// <summary>
//        /// Version.
//        /// </summary>
//        /// <param name="rq">The request.</param>
//        /// <param name="rs">The response.</param>
//        /// <param name="prq">The incoming payload.</param>
//        /// <param name="prs">The outgoing payload.</param>
//        protected override async Task ProcessVersion(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
//            TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            var result = new FileResponseHolder { StatusCode = 408 };
//            //var result = await mStorage.Version(mIdMaker(rq.Key), directory: mDirectory);

//            ProcessOutputKey(rq, rs, result);
//        }
//        #endregion

//        //Not Implemented
//        #region ProcessReadByRef
//        /// <summary>
//        /// Read By Reference
//        /// </summary>
//        /// <param name="rq">The request.</param>
//        /// <param name="rs">The response.</param>
//        /// <param name="prq">The incoming payload.</param>
//        /// <param name="prs">The outgoing payload.</param>
//        protected override async Task ProcessReadByRef(PersistenceRepositoryHolder<K, E> rq, PersistenceRepositoryHolder<K, E> rs,
//            TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            rs.ResponseCode = (int)PersistenceResponse.NotImplemented501;
//            rs.ResponseMessage = "Read by reference not implemented";
//            return;
//        }
//        #endregion
//        #region ProcessDeleteByRef
//        /// <summary>
//        /// Delete by reference request.
//        /// </summary>
//        /// <param name="rq">The request.</param>
//        /// <param name="rs">The response.</param>
//        /// <param name="prq">The incoming payload.</param>
//        /// <param name="prs">The outgoing payload.</param>
//        protected override async Task ProcessDeleteByRef(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
//            TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            rs.ResponseCode = (int)PersistenceResponse.NotImplemented501;
//            rs.ResponseMessage = "Read by reference not implemented";
//            return;
//        }
//        #endregion
//        #region ProcessVersionByRef
//        /// <summary>
//        /// Version by reference.
//        /// </summary>
//        /// <param name="rq">The request.</param>
//        /// <param name="rs">The response.</param>
//        /// <param name="prq">The incoming payload.</param>
//        /// <param name="prs">The outgoing payload.</param>
//        protected override async Task ProcessVersionByRef(PersistenceRepositoryHolder<K, Tuple<K, string>> rq, PersistenceRepositoryHolder<K, Tuple<K, string>> rs,
//            TransmissionPayload prq, List<TransmissionPayload> prs)
//        {
//            rs.ResponseCode = (int)PersistenceResponse.NotImplemented501;
//            rs.ResponseMessage = "Read by reference not implemented";
//            return;
//        }
//        #endregion

//        protected virtual void ProcessOutputEntity(PersistenceRepositoryHolder<K, E> rs, FileResponseHolder holderResponse)
//        {
//            if (holderResponse.IsSuccess)
//            {
//                rs.ResponseCode = holderResponse.StatusCode;
//                rs.Entity = EntityMaker(Encoding.UTF8.GetString(holderResponse.Content));
//                rs.Key = KeyMaker(rs.Entity);
//                rs.Settings.VersionId = mVersion.EntityVersionAsString(rs.Entity);
//            }
//            else
//            {
//                rs.IsTimeout = holderResponse.IsTimeout;
//                rs.ResponseCode = holderResponse.Ex != null ? 500 : holderResponse.StatusCode;

//                if (holderResponse.Ex != null)
//                    Logger.LogException(
//                        string.Format("Error in blob storage persistence {0}-{1}", typeof(E).Name, rs.Key), holderResponse.Ex);
//                else
//                    Logger.LogMessage(LoggingLevel.Warning,
//                        string.Format("Error in blob storage persistence {0}-{1}/{2}-{3}", typeof(E).Name, rs.Key, rs.ResponseCode, rs.ResponseMessage), "BlobStorage");
//            }
//        }

//        protected virtual void ProcessOutputKey(PersistenceRepositoryHolder<K, Tuple<K, string>> rq,
//            PersistenceRepositoryHolder<K, Tuple<K, string>> rs, FileResponseHolder holderResponse)
//        {
//            rs.Key = rq.Key;

//            if (holderResponse.IsSuccess)
//            {
//                rs.ResponseCode = holderResponse.StatusCode;
//                rs.Settings.VersionId = holderResponse.VersionId;
//                rs.Entity = new Tuple<K, string>(rs.Key, holderResponse.VersionId);
//            }
//            else
//            {
//                rs.IsTimeout = holderResponse.IsTimeout;
//                rs.ResponseCode = 404;
//            }
//        }

//        #region Class -> FileResponseHolder
//        /// <summary>
//        /// This is the file response holder.
//        /// </summary>
//        protected class FileResponseHolder
//        {

//            public byte[] Content { get; set; }
//            public bool IsSuccess { get; set; }
//            public int StatusCode { get; set; }
//            public bool IsTimeout { get; set; }
//            public string VersionId { get; set; }

//            public Exception Ex { get; set; }
//        } 
//        #endregion
//    }
//}
