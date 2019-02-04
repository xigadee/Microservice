using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    /// <summary>
    /// This enumeration contains the boundary logging types.
    /// </summary>
    public enum BoundaryEventType
    {
        /// <summary>
        /// This is a poll request.
        /// </summary>
        Poll,
        /// <summary>
        /// This is a boundary transition.
        /// </summary>
        Boundary,
        /// <summary>
        /// This is for messages passing an interface transition such as a WebAPI.
        /// </summary>
        Interface
    }
}
