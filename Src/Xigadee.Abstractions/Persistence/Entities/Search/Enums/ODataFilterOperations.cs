using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// These are the OData filter oparations.
    /// </summary>
    public enum ODataFilterOperations
    {
        /// <summary>
        /// Equal
        /// </summary>
        Equal,
        /// <summary>
        /// Not equal
        /// </summary>
        NotEqual,
        /// <summary>
        /// Less than
        /// </summary>
        LessThan,
        /// <summary>
        /// Less than or equal
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// Greater than
        /// </summary>
        GreaterThan,
        /// <summary>
        /// Greater than or equal
        /// </summary>
        GreaterThanOrEqual
    }
}
