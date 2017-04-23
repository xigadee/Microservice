using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class provides specific directives for communicating with the DocumentDb server.
    /// </summary>
    public class DocumentDbPersistenceCommandPolicy: PersistenceCommandPolicy
    {

        public bool AutoCreateDatabase { get; set; } = true;

        public bool AutoCreateCollections { get; set; } = true;
    }
}
