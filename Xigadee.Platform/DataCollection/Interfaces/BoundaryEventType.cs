using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public enum BoundaryEventType
    {
        /// <summary>
        /// This is a poll request.
        /// </summary>
        Poll,
        /// <summary>
        /// This is a boundary transition.
        /// </summary>
        Boundary
    }
}
