#region using
using IMGroup.Core;
using IMGroup.Core.Persistence;
using IMGroup.Core.Persistence.DocumentDb;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace IMGroup.Microservice
{
    public class JsonResponseHolder:RepositoryHolder, IPersistenceResponseHolder
    {

    }
}
