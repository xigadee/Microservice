
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public class ServiceNotStartedException:Exception
    {
        /// <summary>
        /// Initializes a new instance of the ServiceNotStartedException class.
        /// </summary>
        public ServiceNotStartedException() : base() { }
    }
}
