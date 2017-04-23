using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class contains the change information for the command.
    /// </summary>
    public class MasterJobStateChange
    {
        public MasterJobStateChange(MasterJobState oldState, MasterJobState newState)
        {
            StateOld = oldState;
            StateNew = newState;
        }

        public MasterJobState StateOld { get; protected set; }

        public MasterJobState StateNew { get; protected set; }
    }
}
