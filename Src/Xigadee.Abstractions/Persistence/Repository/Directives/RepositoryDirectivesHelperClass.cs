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
        #region RepositoryCreateMemory(this RepositoryDirective dir)
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
        #endregion
        #region RepositoryCreatePersistenceClient(this RepositoryDirective dir)
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
        #endregion

        #region RepositoryCreatePersistenceServer(this RepositoryDirective dir, RepositoryBase repo)
        /// <summary>
        /// Creates a repository client for the specific type defined in the directive.
        /// </summary>
        /// <param name="dir">The directive.</param>
        /// <param name="repo">The repository.</param>
        /// <returns>Returns the persistence client.</returns>
        public static ICommand RepositoryCreatePersistenceServer(this RepositoryDirective dir, RepositoryBase repo)
        {
            var repoType = typeof(PersistenceServer<,>).MakeGenericType(dir.TypeKey, dir.TypeEntity).GetConstructors();

            var cParamsDef = repoType[0].GetParameters();
            var cParams = new object[cParamsDef.Length];
            cParams[0] = repo;

            var result = (ICommand)repoType[0].Invoke(cParams);

            return result;
        }
        #endregion
    }
}
