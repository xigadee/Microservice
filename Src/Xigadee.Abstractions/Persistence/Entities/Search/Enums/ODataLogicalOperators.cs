using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Xigadee
{
    /// <summary>
    /// These are the OData supported logical operators
    /// </summary>
    public enum ODataLogicalOperators
    {
        /// <summary>
        /// And
        /// </summary>
        OpAnd,
        /// <summary>
        /// Or
        /// </summary>
        OpOr,
        /// <summary>
        /// Xor
        /// </summary>
        OpXor
    }
}
