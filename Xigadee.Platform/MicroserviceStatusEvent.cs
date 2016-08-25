using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This class is used to log status change for the Microservice during start up and stop requests.
    /// </summary>
    public class MicroserviceStatusEventArgs:EventArgs
    {
        public MicroserviceStatusEventArgs(MicroserviceStatusChangeAction status, string title)
        {
            Status = status;
            Title = title;
        }

        public MicroserviceStatusChangeAction Status { get;}

        public string Title { get; }

        public MicroserviceStatusChangeState State { get; set; } =  MicroserviceStatusChangeState.Beginning;

        public MicroserviceStatusChangeException Ex { get; set; }

        public string Debug()
        {
            return $"{Status}: {Title} = {State}";
        }
    }

    public enum MicroserviceStatusChangeAction
    {
        Starting,
        Stopping
    }
    public enum MicroserviceStatusChangeState
    {
        Beginning,
        Completed,
        Failed
    }
}
