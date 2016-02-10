#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This exception is thrown when a message transmit exceeds the default number of tries.
    /// </summary>
    public class TransmitRetryExceededException:Exception
    {
    }
}
