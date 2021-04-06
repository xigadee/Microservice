using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Xigadee
{
    /// <summary>
    /// This helper provides a shortcut for entity upsert.
    /// </summary>
    public static class PersistenceHelper
    {

        #region CreateOrUpdateIfExists<E> ...
        /// <summary>
        /// This repository helper will first try and create an entity, and if this fails because it 
        /// already exists, it will read the entity and then update it.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="repository"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static async Task<RepositoryHolder<Guid, E>> CreateOrUpdateIfExists<E>(this IRepositoryAsync<Guid, E> repository, E entity)
            where E : EntityAuditableBase
        {
            var rsVersion = await repository.Version(entity.Id);

            if (!rsVersion.IsSuccess && rsVersion.ResponseIsNotFound())
            {
                var rsCreate = await repository.Create(entity);

                return rsCreate;
            }
            else if (!rsVersion.IsSuccess)
            {
                var eHolder = new RepositoryHolder<Guid, E>();
                eHolder.ResponseCode = rsVersion.ResponseCode;
                eHolder.Key = rsVersion.Key;
                eHolder.ResponseMessage = rsVersion.ResponseMessage;
                return eHolder;
            }
            //Update the version to the current version to permit the update
            entity.VersionId = new Guid(rsVersion.Entity.Item2);

            var rsUpdate = await repository.Update(entity);

            return rsUpdate;
        }
        #endregion

        #region CreateOrUpdateValidate<E> ...
        /// <summary>
        /// This repository helper will first try and create an entity, and if this fails because it 
        /// already exists, it will read the entity and then update it.
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="repository"></param>
        /// <param name="entity"></param>
        /// <param name="isUpdate">Specifies that this is an update so don't check.</param>
        /// <returns></returns>
        public static async Task<bool> CreateOrUpdateValidate<E>(this IRepositoryAsync<Guid, E> repository, E entity
            , bool isUpdate = false)
            where E : EntityAuditableBase
        {
            if (!isUpdate)
            {
                var rsCreate = await repository.Create(entity);
                if (rsCreate.IsSuccess)
                    return true;

                ////Check that it already exist.
                //if (rsCreate.ResponseCode != 408)
                //    return false;

                var rsRead = await repository.Read(entity.Id);

                if (!rsRead.IsSuccess)
                    return false;

                entity.VersionId = rsRead.Entity.VersionId;
            }

            var rsUpdate = await repository.Update(entity);

            return rsUpdate.IsSuccess;
        }
        #endregion

        #region CreateOrUpdate<E> ...
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="repository"></param>
        /// <param name="entity"></param>
        /// <param name="isUpdate"></param>
        /// <param name="onUpdate">This action is called before the entity is updated</param>
        /// <returns></returns>
        public static Task<RepositoryHolder<Guid, E>> CreateOrUpdate<E>(this IRepositoryAsync<Guid, E> repository, E entity, bool isUpdate, Action<E> onUpdate = null)
            where E : EntityAuditableBase
        {
            if (isUpdate)
            {
                onUpdate?.Invoke(entity);
                return repository.Update(entity);
            }
            else
                return repository.Create(entity);
        } 
        #endregion

        #region CreateOrUpdateForce<E>(this IRepositoryAsync<Guid, E> repository, E entity, bool? isUpdate = null, bool doNotUpdateIfExists = false)

        /// <summary>
        /// This helper method is used to force the update of an entity if it already exists.
        /// </summary>
        /// <typeparam name="E">The entity type.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="isUpdate">Flag that indicates that this is expected to be an update</param>
        /// <param name="doNotUpdateIfExists">Don't update the entity if it already exists</param>
        /// <returns>Returns the repository response.</returns>
        public static async Task<RepositoryHolder<Guid, E>> CreateOrUpdateForce<E>(this IRepositoryAsync<Guid, E> repository, E entity, bool? isUpdate = null, bool doNotUpdateIfExists = false)
            where E : EntityAuditableBase
        {
            if (!isUpdate.HasValue)
            {
                var rsRead = await repository.Read(entity.Id);

                if (!rsRead.IsSuccess && rsRead.ResponseIsNotFound())
                {
                    isUpdate = false;
                }
                else if (!rsRead.IsSuccess)
                    return rsRead;
                else
                {
                    if (doNotUpdateIfExists)
                        return rsRead;
                    isUpdate = true;
                    if (entity.VersionId != rsRead.Entity.VersionId)
                        entity.VersionId = rsRead.Entity.VersionId;
                }
            }

            return await repository.CreateOrUpdate(entity, isUpdate.Value);
        } 
        #endregion
    }
}
