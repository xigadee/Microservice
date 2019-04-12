using System;
using System.Collections.Generic;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// This is the sql repository directives helper.
    /// </summary>
    public static class SqlRespositoryDirectivesHelper
    {
        /// <summary>
        /// This method creates a SqlJson repository from the directive declaration.
        /// </summary>
        /// <param name="dir">The directive.</param>
        /// <param name="sqlConnection">The Sql conenction.</param>
        /// <returns>Returns the repository.</returns>
        public static RepositoryBase RepositoryCreateSqlJson(RepositoryDirective dir, string sqlConnection)
        {
            var repoType = typeof(RepositorySqlJson<,>).MakeGenericType(dir.TypeKey, dir.TypeEntity).GetConstructors();

            var cParamsDef = repoType[0].GetParameters();
            var cParams = new object[cParamsDef.Length];
            cParams[0] = sqlConnection;

            var result = (RepositoryBase)repoType[0].Invoke(cParams);

            return result;
        }
    }
}
