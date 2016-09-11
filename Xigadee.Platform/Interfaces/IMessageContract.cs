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
    /// This is the base interface used for message contracts.
    /// </summary>
    public interface IMessageContract
    {

    }

    public interface IMessageContractPayload<P>:IMessageContract
    {

    }
}
