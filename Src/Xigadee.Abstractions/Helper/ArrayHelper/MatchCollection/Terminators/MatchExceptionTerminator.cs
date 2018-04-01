using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TMatch"></typeparam>
    public class MatchExceptionTerminator<TSource, TMatch> : MatchTerminator<TSource, TMatch>
    {
        #region Constructor
        public MatchExceptionTerminator(IEnumerable<TMatch> Terminator)
            : base(Terminator, false)
        {

        }

        public MatchExceptionTerminator(IEnumerable<TMatch> Terminator, bool CanScan,
            Func<TSource, MatchTerminatorResult, MatchTerminatorStatus> Predicate,
            Func<MatchTerminatorResult, Queue<TSource>, TSource, long, bool> PredicateTerminator)
            : base(Terminator, CanScan, Predicate, PredicateTerminator)
        {

        }
        #endregion // Constructor

        #region Validate(TSource item, MatchTerminatorResult currentResult)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="currentResult"></param>
        /// <returns></returns>
        protected override MatchTerminatorStatus Validate(TSource item, MatchTerminatorResult currentResult)
        {
            bool result = Terminator.Contains(t => t.Equals(item));

            return result ? MatchTerminatorStatus.Fail : MatchTerminatorStatus.SuccessNoLength;
        }
        #endregion  
    }
}
