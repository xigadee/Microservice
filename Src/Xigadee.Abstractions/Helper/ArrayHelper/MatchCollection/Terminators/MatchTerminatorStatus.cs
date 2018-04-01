using System;

namespace Xigadee
{
    /// <summary>
    /// This enumeration is used to defined the match status.
    /// </summary>
    [Flags]
    public enum MatchTerminatorStatus
    {
        /// <summary>
        /// 
        /// </summary>
        NotSet = 0,
        /// <summary>
        /// 
        /// </summary>
        Fail = 1,
        /// <summary>
        /// 
        /// </summary>
        FailNoLength = 9,
        /// <summary>
        /// 
        /// </summary>
        Success = 2,
        /// <summary>
        /// 
        /// </summary>
        SuccessReset = 34,
        /// <summary>
        /// 
        /// </summary>
        SuccessNoLength = 10,
        /// <summary>
        /// 
        /// </summary>
        SuccessNoLengthReset = 42,
        /// <summary>
        /// 
        /// </summary>
        SuccessPartial = 4,
        /// <summary>
        /// 
        /// </summary>
        NoLength = 8,
        /// <summary>
        /// 
        /// </summary>
        Exception = 16,
        /// <summary>
        /// 
        /// </summary>
        Reset = 32
    }
}
