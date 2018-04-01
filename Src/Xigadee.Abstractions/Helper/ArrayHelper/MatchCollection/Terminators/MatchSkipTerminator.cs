using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This terminator is used to skip items in a collection up to a certain criteria.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TMatch"></typeparam>
    public class MatchSkipTerminator<TSource, TMatch> : MatchTerminator<TSource, TMatch>
    {
        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Terminator"></param>
        /// <param name="CanScan"></param>
        public MatchSkipTerminator(IEnumerable<TMatch> Terminator, bool CanScan)
            : base(Terminator, CanScan)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Terminator"></param>
        /// <param name="CanScan"></param>
        /// <param name="Predicate"></param>
        /// <param name="PredicateTerminator"></param>
        public MatchSkipTerminator(IEnumerable<TMatch> Terminator, bool CanScan,
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

            return result ? MatchTerminatorStatus.SuccessPartial : MatchTerminatorStatus.SuccessNoLength;
        }
        #endregion  
    }
}
