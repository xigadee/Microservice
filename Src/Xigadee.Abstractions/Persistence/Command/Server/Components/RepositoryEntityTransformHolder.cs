using System;

namespace Xigadee
{
    /// <summary>
    /// This holder holds the repository.
    /// </summary>
    /// <typeparam name="K">The key type.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public class RepositoryEntityTransformHolder<K, E> : EntityTransformHolder<K, E>
        where K : IEquatable<K>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryEntityTransformHolder{K, E}"/> class.
        /// </summary>
        /// <param name="repo">The repository.</param>
        public RepositoryEntityTransformHolder(IRepositoryAsyncServer<K, E> repo) : base(false)
        {
            Repository = repo;
        }

        #region Declarations
        /// <summary>
        /// This the is the repository that is used to store and retrieve entities.
        /// </summary>
        public IRepositoryAsyncServer<K, E> Repository { get; }
        #endregion

        public override VersionPolicy<E> Version { get => Repository.VersionPolicy; }

        public override Func<E, K> KeyMaker {
            get => (e) =>
            {
                K key;
                if (Repository.TryKeyExtract(e, out key))
                    return key;
                return default(K);
            };
        }
    }
}
