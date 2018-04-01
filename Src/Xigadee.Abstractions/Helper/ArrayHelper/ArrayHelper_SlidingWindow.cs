using System;
using System.Collections.Generic;

namespace Xigadee
{
    public static partial class ArrayHelper
    {
        #region ValidateCollectionSlidingWindow<TSource, TMatch>(MatchCollectionState<TSource, TMatch> state, bool deQueue)
        /// <summary>
        /// This method validates the state sliding window.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TMatch"></typeparam>
        /// <param name="state">The match collection state.</param>
        /// <param name="deQueue"></param>
        /// <returns>Returns the updated match state.</returns>
        private static MatchCollectionState<TSource, TMatch> ValidateCollectionSlidingWindow<TSource, TMatch>(
              MatchCollectionState<TSource, TMatch> state
            , bool deQueue)
        {
            //Remove the first item from the collection.
            if (deQueue)
            {
                state.SlidingWindow.Dequeue();
                state.Length++;
                state.Length -= state.SlidingWindow.Count;
            }
#if (DEBUG)
            MatchStateDebugLog(state, "Recurse");
#endif
            //Ok, check whether the queue is empty. This can happen when the queue only contained
            //1 item and was called recursively.
            if (state.SlidingWindow.Count == 0)
            {
                state.Status = MatchTerminatorStatus.NotSet;
                return state;
            }

            Queue<TSource> window = state.SlidingWindow;

            state.CurrentEnumerator.Reset();
            state.CurrentEnumerator.MoveNext();
            state.SlidingWindow = new Queue<TSource>();
            //OK, we recursively call the window to allow the queue to 
            //be processed.

            return window.MatchCollection(state);
        }
        #endregion
        #region ValidateSlidingWindow<TSource, TMatch>(MatchState<TSource> state, IEnumerator<TMatch> matchEnum, Func<TSource, TMatch, bool> predicate)
        /// <summary>
        /// This method validates the sliding windows of previous records. This is needed because
        /// there may be partial matches in the previous array records. This is especially important
        /// when the match array is long.
        /// </summary>
        /// <typeparam name="TSource">The source array type.</typeparam>
        /// <typeparam name="TMatch">The match array type.</typeparam>
        /// <param name="state">The current match state.</param>
        /// <param name="matchEnum">The match enumerator.</param>
        /// <param name="predicate">The predicate used to validate the source and match items.</param>
        /// <returns>The match position or 0 if there is no match.</returns>
        private static int ValidateSlidingWindow<TSource, TMatch>(
              MatchState<TSource> state
            , IEnumerator<TMatch> matchEnum
            , Func<TSource, TMatch, bool> predicate)
        {
            int posMatch;

            //Remove the start of the previous point.
            state.SlidingWindow.Dequeue();

            //OK, no match so reset the match buffer to its default position
            while (state.SlidingWindow.Count > 0 && !SlidingWindowMatch(predicate, state.SlidingWindow, matchEnum))
                state.SlidingWindow.Dequeue();

            posMatch = state.SlidingWindow.Count;
            //Reset the match enumerator to its initial position.
            if (posMatch == 0)
            {
                matchEnum.Reset();
                matchEnum.MoveNext();
            }

            return posMatch;
        }
        #endregion // ValidateSlidingWindow
        #region SlidingWindowMatch
        /// <summary>
        /// This method matches the sliding window with the match.
        /// </summary>
        /// <typeparam name="TSource">The source array type.</typeparam>
        /// <typeparam name="TMatch">The match array type.</typeparam>
        /// <param name="predicate">The predicate used to validate the source and match items.</param>
        /// <param name="queue">The sliding window queue.</param>
        /// <param name="matchEnum">The match enumerator.</param>
        /// <returns>Returns true if there is a partial match.</returns>
        private static bool SlidingWindowMatch<TSource, TMatch>(
              Func<TSource, TMatch, bool> predicate
            , Queue<TSource> queue
            , IEnumerator<TMatch> matchEnum)
        {
            matchEnum.Reset();
            matchEnum.MoveNext();

            TMatch match;

            foreach (TSource item in queue)
            {
                match = matchEnum.Current;
                if (!predicate(item, match))
                {
                    return false;
                }
                //Ok, move to the next item in the match enumeration.
                if (!matchEnum.MoveNext())
                    throw new Exception("ArrayHelper/SlidingWindowMatch --> matchEnum.MoveNext() was not successful. This should not happen.");
            }

            return true;
        }
        #endregion // SlidingWindowMatch
    }
}
