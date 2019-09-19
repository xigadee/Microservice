using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xigadee;
namespace Tests.Xigadee.Azure.Sql
{
    public class RepositoryJsonTest1 : RepositorySqlJson<Guid, Test1>
    {
        public RepositoryJsonTest1(string sqlConnection, Func<Test1, Guid> keyMaker
            , ISqlStoredProcedureResolver spNamer = null
            , Func<Test1, IEnumerable<Tuple<string, string>>> referenceMaker = null
            , Func<Test1, IEnumerable<Tuple<string, string>>> propertiesMaker = null
            , VersionPolicy<Test1> versionPolicy = null, RepositoryKeyManager<Guid> keyManager = null) 
            : base(sqlConnection, keyMaker, spNamer, referenceMaker, propertiesMaker, versionPolicy, keyManager)
        {
        }
    }


}
