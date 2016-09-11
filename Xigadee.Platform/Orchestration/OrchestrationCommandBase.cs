using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the base class for commands that implement orchestration functionality .
    /// </summary>
    /// <typeparam name="O">This is the flow component that the command is concerned with.</typeparam>
    /// <typeparam name="S">This is the statistics class that reports the status of the component.</typeparam>
    /// <typeparam name="P">This class holds the policy for the component.</typeparam>
    public abstract class OrchestrationCommandBase<O, S, P>: CommandBase<S, P>
        where O : OrchestrationFlowComponentBase
        where S : OrchestrationCommandStatistics, new()
        where P : OrchestrationCommandPolicy, new()
    {
    }
}
