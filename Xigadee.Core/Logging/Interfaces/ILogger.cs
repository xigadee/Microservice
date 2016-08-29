using System;
using System.Threading.Tasks;

namespace Xigadee
{
    public interface ILogger
    {
        Task Log(LogEvent logEvent);
    }
}
