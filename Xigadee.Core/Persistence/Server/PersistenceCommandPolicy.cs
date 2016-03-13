using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the default command policy for the persistence class.
    /// </summary>
    public class PersistenceCommandPolicy:CommandPolicy
    {

        /// <summary>
        /// Persistence managers should not be master jobs by default.
        /// </summary>
        public override bool MasterJobEnabled { get { return false; } }
    }
}
