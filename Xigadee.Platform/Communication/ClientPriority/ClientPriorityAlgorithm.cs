#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This abstract class is the base class to allocate polling slots to the listener collection.
    /// </summary>
    public abstract class ClientPollSlotAllocationAlgorithm
    {

    }

    /// <summary>
    /// This is the default slot allocation algorithm.
    /// </summary>
    public class DefaultClientPollSlotAllocationAlgorithm: ClientPollSlotAllocationAlgorithm
    {

    }
}
