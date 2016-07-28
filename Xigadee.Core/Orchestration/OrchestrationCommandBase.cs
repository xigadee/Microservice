using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public abstract class OrchestrationCommandBase<O, S, P>: CommandBase<S, P>
        where O : OrchestrationFlowComponentBase
        where S : OrchestrationCommandStatistics, new()
        where P : OrchestrationCommandPolicy, new()
    {
    }
}
