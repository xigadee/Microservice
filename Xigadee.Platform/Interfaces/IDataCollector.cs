using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This is the combined interface that allow specific components to centralise all the logging capabilities in to single class.
    /// </summary>
    public interface IDataCollector: IBoundaryLogger, ILogger, IEventSource, ITelemetry, IServiceOriginator
    {

    }
}
