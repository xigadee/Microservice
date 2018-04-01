using System;
using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TMatch"></typeparam>
    public class MatchSequenceTerminator<TSource, TMatch> : MatchTerminator<TSource, TMatch>
    {
        #region Constructor
        /// <summary>
        /// go8
        /// </summary>
        /// <param name="Terminator">The terminator enumeration.</param>
        /// <param name="CanScan"></param>
        public MatchSequenceTerminator(IEnumerable<TMatch> Terminator, bool CanScan)
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
        public MatchSequenceTerminator(IEnumerable<TMatch> Terminator
            , bool CanScan
            , Func<TSource, MatchTerminatorResult, MatchTerminatorStatus> Predicate
            , Func<MatchTerminatorResult, Queue<TSource>, TSource, long, bool> PredicateTerminator
            )
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
            bool result = item.Equals(CurrentTerminator.Current);

            if (!result)
                return MatchTerminatorStatus.Fail;

            return CurrentTerminator.MoveNext() ?
                MatchTerminatorStatus.SuccessPartial : MatchTerminatorStatus.Success;
        }
        #endregion  

    }
}
