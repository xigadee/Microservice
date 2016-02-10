#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This abstract class provides a base framework for interactive with an entity store.
    /// </summary>
    /// <typeparam name="K">The entity key type.</typeparam>
    /// <typeparam name="E">The entity.</typeparam>
    public abstract class ProviderBase<K, E> : IRepository<K, E>
        where K : IEquatable<K>
    {
        #region Declarations
        /// <summary>
        /// This function returns a key from an entity;
        /// </summary>
        protected Func<E, K> mKeyMaker; 
        #endregion

        #region Constructor
        /// <summary>
        /// This is the expanded constructor where a collection of items are passed in manually.
        /// </summary>
        /// <param name="coll">The data collection.</param>
        /// <param name="keyMaker">The key maker function.</param>
        public ProviderBase(Func<E, K> keyMaker)
        {
            mKeyMaker = keyMaker;
        }
        #endregion

        #region Create(E entity)
        /// <summary>
        /// This is the generic SQL entity create code.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        public virtual void Create(E entity)
        {
            RepositoryOptions status = null;
            Create(entity, ref status);
            //For backwards compatibility.
            if (status.IsFaulted)
                throw status.Ex;
        } 
        #endregion
        #region E Read(K id)
        /// <summary>
        /// This method is used to read an entity from the system.
        /// </summary>
        /// <param name="id">The entity key.</param>
        /// <returns>Returns the entity or null if the entity cannot be found.</returns>
        public virtual E Read(K id)
        {
            RepositoryOptions status = null;
            var entity = Read(id, ref status);

            //For backwards compatibility.
            if (status.IsFaulted)
                throw status.Ex;

            return entity;
        } 
        #endregion
        #region Update(E entity)
        /// <summary>
        /// This method is used to update an existing entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public virtual void Update(E entity)
        {
            RepositoryOptions status = null; ;
            Update(entity, ref status);
            //For backwards compatibility.
            if (status.IsFaulted)
                throw status.Ex;
        } 
        #endregion
        #region Delete(K id)
        /// <summary>
        /// This method is used to delete an entity or mark it as deleted.
        /// </summary>
        /// <param name="id">The entity key.</param>
        public virtual void Delete(K id)
        {
            RepositoryOptions status = null;
            Delete(id, ref status);
            //For backwards compatibility.
            if (status.IsFaulted)
                throw status.Ex;
        } 
        #endregion


        public abstract void Create(E entity, ref RepositoryOptions status);


        public abstract E Read(K id, ref RepositoryOptions status);


        public abstract void Update(E entity, ref RepositoryOptions status);


        public abstract void Delete(K id, ref RepositoryOptions status);

    }
}
