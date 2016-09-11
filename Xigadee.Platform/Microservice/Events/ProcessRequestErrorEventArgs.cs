using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ProcessRequestErrorEventArgs: ProcessRequestUnresolvedEventArgs
    {
        public Exception Ex { get; set; }
    }
}
