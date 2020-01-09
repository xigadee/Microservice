using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This class provides the basic constructor support.
    /// </summary>
    public static class SqlPersistenceHelper
    {

        #region RepositoryCreateSqlJson2(RepositoryDirective dir)
        /// <summary>
        /// This class is responsible for setting the repository as SQLJson backed based on the type of entity.
        /// Override this member to intercept this behaviour when you require a difference repo.
        /// </summary>
        /// <param name="dir">The repository directive.</param>
        /// <param name="sqlConnection">The sqlConnection.</param>
        /// <param name="spNamer">The stored procedure namer</param>
        /// <param name="signaturePolicy">The signature policy.</param>
        /// <param name="repositoryType">The repository type. This must have the same constructor format for this method to work.</param>
        /// <returns>Returns the repository.</returns>
        public static RepositoryBase RepositoryCreateSqlJson2(this RepositoryDirective dir
            , string sqlConnection
            , ISqlStoredProcedureResolver spNamer = null
            , ISignaturePolicy signaturePolicy = null
            , Type repositoryType = null)
            => RepositoryCreateSqlJson2(dir.TypeKey, dir.TypeEntity, sqlConnection, spNamer, signaturePolicy, repositoryType);

        /// <summary>
        /// This method creates a repository using the type parameters.
        /// </summary>
        /// <param name="keyType"></param>
        /// <param name="entityType"></param>
        /// <param name="sqlConnection">The sqlConnection.</param>
        /// <param name="spNamer">The stored procedure namer</param>
        /// <param name="signaturePolicy">The signature policy.</param>
        /// <param name="repositoryType">The repository type. This must have the same constructor format for this method to work.</param>
        /// <returns></returns>
        public static RepositoryBase RepositoryCreateSqlJson2(Type keyType, Type entityType
            , string sqlConnection
            , ISqlStoredProcedureResolver spNamer = null
            , ISignaturePolicy signaturePolicy = null
            , Type repositoryType = null)
        {
            repositoryType = repositoryType ?? typeof(RepositorySqlJson2<,>);
            ConstructorInfo[] repoType = repositoryType.MakeGenericType(keyType, entityType).GetConstructors();

            var cParamsDef = repoType[0].GetParameters();

            var cParams = new object[cParamsDef.Length];

            cParams[0] = sqlConnection;
            cParams[2] = spNamer;
            cParams[7] = signaturePolicy;

            var result = (RepositoryBase)repoType[0].Invoke(cParams);

            return result;
        }

        #endregion
    }
}
