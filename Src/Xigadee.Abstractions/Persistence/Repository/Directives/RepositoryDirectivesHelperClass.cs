using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the support classs.
    /// </summary>
    public static class RepositoryDirectiveHelperClass
    {
        /// <summary>
        /// This method creates a memory repository for the specific type defined in the directive.
        /// </summary>
        /// <param name="dir">The directive.</param>
        /// <returns>Returns the memory repository.</returns>
        public static RepositoryBase RepositoryCreateMemory(this RepositoryDirective dir)
        {
            var repoType = typeof(RepositoryMemory<,>).MakeGenericType(dir.TypeKey, dir.TypeEntity).GetConstructors();

            var result = (RepositoryBase)repoType[0].Invoke(new object[repoType[0].GetParameters().Length]);

            return result;
        }
        /// <summary>
        /// Creates a repository client for the specific type defined in the directive.
        /// </summary>
        /// <param name="dir">The directive.</param>
        /// <returns>Returns the persistence client.</returns>
        public static PersistenceClientBase RepositoryCreatePersistenceClient(this RepositoryDirective dir)
        {
            var repoType = typeof(PersistenceClient<,>).MakeGenericType(dir.TypeKey, dir.TypeEntity).GetConstructors();

            var result = (PersistenceClientBase)repoType[0].Invoke(new object[repoType[0].GetParameters().Length]);

            return result;
        }
    }
}
