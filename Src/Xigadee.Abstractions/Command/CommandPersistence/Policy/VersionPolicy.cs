using System;
using System.Collections.Generic;
namespace Xigadee
{
    #region VersionPolicy
    /// <summary>
    /// The default empty version policy class.
    /// </summary>
    /// <seealso cref="Xigadee.PolicyBase" />
    public abstract class VersionPolicy : PolicyBase
    {
    }
    #endregion
    #region VersionPolicy<E>
    /// <summary>
    /// This class handles version support for the entity.
    /// </summary>
    /// <typeparam name="E">The entity type.</typeparam>
    public class VersionPolicy<E> : VersionPolicy
    {
        #region Declarations
        private readonly Func<E, string> mEntityVersionAsString;

        private readonly Action<E> mEntityVersionUpdate;

        /// <summary>
        /// This is the meta data key set for the entity version.
        /// </summary>
        public readonly KeyValuePair<string, string> VersionJsonMetadata = new KeyValuePair<string, string>("$microservice.version", "$microservice.version");
        #endregion
        #region Constructor
        /// <summary>
        /// This is the version policy constructor.
        /// </summary>
        /// <param name="entityVersionAsString">This function returns the entity version as a string.</param>
        /// <param name="entityVersionUpdate">This action updates the entity version with a new value.</param>
        /// <param name="supportsArchiving">This boolean method specifies whether the old version should be archived.</param>
        public VersionPolicy(Func<E, string> entityVersionAsString = null, Action<E> entityVersionUpdate = null, bool supportsArchiving = false)
        {
            mEntityVersionAsString = entityVersionAsString ?? ((e) => (string)null);
            mEntityVersionUpdate = entityVersionUpdate ?? ((e) => { });
            SupportsVersioning = entityVersionAsString != null;
            SupportsOptimisticLocking = SupportsVersioning && entityVersionUpdate != null;
            SupportsArchiving = supportsArchiving;
        }
        #endregion

        #region EntityVersionUpdate(E entity)
        /// <summary>
        /// This method updates the version id of the entity and returns the new version id as a string.
        /// </summary>
        /// <param name="entity">The entity to modify.</param>
        /// <returns>Returns a string representation of the version id.</returns>
        public virtual string EntityVersionUpdate(E entity)
        {
            mEntityVersionUpdate(entity);
            return mEntityVersionAsString(entity);
        }
        #endregion
        #region EntityVersionAsString(E entity)
        /// <summary>
        /// This method returns the entity key as a string.
        /// </summary>
        /// <param name="entity">The entity to process.</param>
        /// <returns>Returns a string representation of the version id.</returns>
        public virtual string EntityVersionAsString(E entity)
        {
            return mEntityVersionAsString(entity);
        }
        #endregion

        /// <summary>
        /// This property specifies whether the persistence agent should implement versioning.
        /// </summary>
        public bool SupportsVersioning { get; private set; }
        /// <summary>
        /// This property specifies whether the persistence agent should implement optimistic locking.
        /// </summary>
        public bool SupportsOptimisticLocking { get; private set; }
        /// <summary>
        /// This property specifies whether the persistence agent should implement archiving after an update or a delete.
        /// </summary>
        public bool SupportsArchiving { get; private set; }

        /// <summary>
        /// Implicitly converts a string in to a resource profile.
        /// </summary>
        /// <param name="t">The name of the resource profile.</param>
        public static implicit operator VersionPolicy<E>(ValueTuple<Func<E, string>> t)
        {
            return new VersionPolicy<E>(t.Item1);
        }
        /// <summary>
        /// Implicitly converts a string in to a resource profile.
        /// </summary>
        /// <param name="t">The name of the resource profile.</param>
        public static implicit operator VersionPolicy<E>(ValueTuple<Func<E, string>, Action<E>> t)
        {
            return new VersionPolicy<E>(t.Item1, t.Item2);
        }
        /// <summary>
        /// Implicitly converts a string in to a resource profile.
        /// </summary>
        /// <param name="t">The name of the resource profile.</param>
        public static implicit operator VersionPolicy<E>(ValueTuple<Func<E, string>, Action<E>, bool> t)
        {
            return new VersionPolicy<E>(t.Item1, t.Item2, t.Item3);
        }
    } 
    #endregion
}
