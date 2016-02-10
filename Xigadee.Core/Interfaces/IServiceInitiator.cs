using System;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This interface is implemented by processes that initiate actions that call in to the service,
    /// such as message listeners and job poll processes.
    /// </summary>
    public interface IServiceInitiator:IService
    {
        Action<IService, TransmissionPayload> Dispatcher { get; set; }
    }
}
