using System;
using System.Threading.Tasks;
namespace Xigadee
{
    /// <summary>
    /// This interface is used to provide generic support for event sources for components
    /// </summary>
    public interface IEventSourceComponent: IEventSource
    {
        /// <summary>
        /// This is the name of the component.
        /// </summary>
        string Name { get; }
    }

    public interface IEventSource
    {
        Task Write<K,E>(string originatorId, EventSourceEntry<K,E> entry, DateTime? utcTimeStamp = null, bool sync = false);

    }
}
