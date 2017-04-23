using System;
using System.Collections.Generic;

namespace Xigadee
{
    public interface IServiceEventSource
    {
        /// <summary>
        /// This is the event source for the communication layer.
        /// </summary>
        IEventSource EventSource { get; set; }
    }
}
