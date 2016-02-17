#region using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is a default repositry interface for entities within the system.
    /// </summary>
    /// <typeparam name="K">The entity key object.</typeparam>
    /// <typeparam name="E">The entity type.</typeparam>
    public interface IRepository<K,E>
    {
        [Obsolete("Use RepositoryOptions going forward for better control.", false)]
        void Create(E entity);

        void Create(E entity, ref RepositoryOptions status);

        [Obsolete("Use RepositoryOptions going forward for better control.", false)]
        E Read(K id);

        E Read(K id, ref RepositoryOptions status);

        [Obsolete("Use RepositoryOptions going forward for better control.", false)]
        void Update(E entity);

        void Update(E entity, ref RepositoryOptions status);

        [Obsolete("Use RepositoryOptions going forward for better control.", false)]
        void Delete(K id);

        void Delete(K id, ref RepositoryOptions status);
    }
}
