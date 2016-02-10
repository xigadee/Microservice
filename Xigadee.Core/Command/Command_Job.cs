#region using
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This command is the base implementation that allows multiple commands to be handled within a single container.
    /// </summary>
    public abstract partial class CommandBase<S>
    {
    }
}
