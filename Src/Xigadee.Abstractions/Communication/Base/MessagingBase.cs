using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
namespace Xigadee
{
    /// <summary>
    /// This is the generic messaging holder for a particular communication protocol.
    /// </summary>
    /// <typeparam name="C">The client type.</typeparam>
    /// <typeparam name="M">The client message type.</typeparam>
    /// <typeparam name="H">The client-holder type.</typeparam>
    [DebuggerDisplay("{GetType().Name}: {ChannelId}|{MappingChannelId}@{Status} [{ComponentId}]")]
    public partial class MessagingBase<C, M, H> : MessagingServiceBase<C, M, H>, IListener, ISender
        where H : ClientHolder<C, M>, new()
        where C : class
    {
    }
}
