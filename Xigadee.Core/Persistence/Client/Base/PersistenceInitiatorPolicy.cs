using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used for persistence based shortcuts.
    /// </summary>
    public class PersistenceInitiatorPolicy: CommandPolicy
    {
        public override bool OutgoingRequestsEnabled { get; set; } = true;
    }
}
