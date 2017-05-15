#region Copyright
// Copyright Hitachi Consulting
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xigadee
{
    public static partial class ArrayHelper
    {
        #region MatchCollection<TSource, TMatch>(this IEnumerable<TSource> source, MatchCollectionState<TSource, TMatch> matchCollectionState)
        /// <summary>
        /// This method matches the source enumeration against the state collection, this is to allow pattern
        /// matching over multiple byte blocks.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TMatch">The match type.</typeparam>
        /// <param name="source">The enumerable source.</param>
        /// <param name="state">The incoming match state.</param>
        /// <returns>Returns the updated match state.</returns>
        public static MatchCollectionState<TSource, TMatch> MatchCollection<TSource, TMatch>(
            this IEnumerable<TSource> source
            , MatchCollectionState<TSource, TMatch> state)
        {
            IEnumerator<TSource> sourceEnum = source.GetEnumerator();
            sourceEnum.MoveNext();
            return sourceEnum.MatchCollection(state);
        }
        #endregion
        #region MatchCollection<TSource, TMatch>(this IEnumerator<TSource> sourceEnum, MatchCollectionState<TSource, TMatch> state)
        /// <summary>
        /// This method matches the source enumeration against the state collection, this is to allow pattern
        /// matching over multiple byte blocks.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TMatch">The match type.</typeparam>
        /// <param name="sourceEnum">The source enumeration data.</param>
        /// <param name="state">The incoming match state.</param>
        /// <returns>Returns the updated match state.</returns>
        public static MatchCollectionState<TSource, TMatch> MatchCollection<TSource, TMatch>(
            this IEnumerator<TSource> sourceEnum
            , MatchCollectionState<TSource, TMatch> state)
        {
            if (state == null)
                throw new ArgumentNullException("state cannot be null.");

#if (DEBUG)
            state.DebugTraceRecursion++;
#endif
            try
            {
#if (DEBUG) 
                MatchStateDebugLog(state,"Entry"); 
#endif
                //Ok, have we already matched in which case return the current state unchanged.
                if (state.Status.HasValue && ((state.Status.Value & MatchTerminatorStatus.Success) > 0))
                    return state;

                //Initialize the state.
                if (!state.Status.HasValue)
                {
                    state.Start = state.Length;
                    state.Status = MatchTerminatorStatus.NotSet;
                }

                //OK, get the MatchTerminator current enumerator or create a new one if it is null.
                IEnumerator<MatchTerminator<TSource, TMatch>> currentEnum = state.ActualEnumerator;

                //Check whether there is any data from the sliding window and if so process it first.
                if (state.SlidingWindow.Count > 0 && state.Status == MatchTerminatorStatus.NotSet)
                    state = ValidateCollectionSlidingWindow(state, false);

                //If the sliding window data has completed the match, then exit
                if ((state.Status & MatchTerminatorStatus.Success) > 0)
                    return state;

                //Ok, let's start the match.
                MatchTerminatorResult result;
                bool reset = false;
                do
                {
                    MatchTerminator<TSource, TMatch> term = currentEnum.Current;
                    result = term.Match(sourceEnum, state.SlidingWindow, state.Length - state.Start);

                    reset = (result.Status & MatchTerminatorStatus.Reset) > 0;
#if (DEBUG)
                    if (state.DebugTrace)
                        state.DebugTraceCollection.Add(
                            string.Format("Match ({0})={1} [{2:X}]", term.GetType().Name, result.Status, state.Length));
#endif
                    state.IsTerminator |= result.IsTerminator;

                    switch (result.Status)
                    {
                        case MatchTerminatorStatus.Fail:
                            term.Reset();
                            state.Length += result.Length - 1;// +state.SlidingWindow.Count - 1;
                            if (state.SlidingWindow.Count > 0)
                            {
                                int oldLength = state.Length;
                                //Dispose of the first item, and process the queue.
                                state = ValidateCollectionSlidingWindow<TSource, TMatch>(state, true);
                                //Ok, we need to check whether we have a partial match in the enqueued bytes.
                                currentEnum = state.CurrentEnumerator;
                            }

                            if (state.Status != MatchTerminatorStatus.SuccessPartial)
                                currentEnum.Reset();

                            break;

                        case MatchTerminatorStatus.SuccessPartial:
                            //Ok, we have a partial match but have reached the end of sourceEnum, so return
                            //and wait for the next piece.
                            state.Status = MatchTerminatorStatus.SuccessPartial;
                            state.Length += result.Length;
                            if (state.MatchPosition == -1)
                                state.MatchPosition = state.Length - state.SlidingWindow.Count;
                            return state;

                        case MatchTerminatorStatus.NotSet:
                            //Ok, we have scanned to the end of the array and not found a match.
                            state.MatchPosition = -1;
                            state.Length += result.Length;
                            return state;

                        case MatchTerminatorStatus.SuccessNoLength:
                        case MatchTerminatorStatus.SuccessNoLengthReset:
                            term.Reset();
                            state.Length += result.Length;
                            if (state.MatchPosition == -1)
                                state.MatchPosition = state.Length - state.SlidingWindow.Count;

                            if (reset)
                            {
                                currentEnum.Reset();
                                currentEnum.MoveNext();
                            }


                            break;

                        case MatchTerminatorStatus.Success:
                        case MatchTerminatorStatus.SuccessReset:
                            term.Reset();
                            state.Length += result.Length;
                            if (state.MatchPosition == -1)
                                state.MatchPosition = state.Length - state.SlidingWindow.Count;

                            if (reset)
                            {
                                currentEnum.Reset();
                                currentEnum.MoveNext();
                            }
                            //Ok, we are successful, so we need to move on to the next step.

                            if (!result.CanContinue || reset)
                            {
                                //We have reached the end of the byte stream so we need to check whether we have
                                //actually terminated.
                                bool moreTerminatorParts = reset ? false : currentEnum.MoveNext();
                                if (!state.IsTerminator && moreTerminatorParts)
                                {
                                    //OK, we only have a partial match as the terminator flag is not set and we have more parts
                                    //of the terminator to process.
                                    state.Status = MatchTerminatorStatus.SuccessPartial;
                                }
                                else
                                {
                                    //Ok, either the termination flag is set, meaning the terminator chars match a specific character
                                    //or there are no more parts of the terminator to match.
                                    state.Status = MatchTerminatorStatus.Success;
                                    state.SlidingWindow.Clear();
                                }

                                return state;
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException("Unknown MatchTerminatorStatus value.");
                    }
                }
                while (result.CanContinue && currentEnum.MoveNext() && !reset);

                //OK, we have completed. Either we have failed or succeeded
                state.Status = result.Status;

                //OK, time to do some tidy up.
                if ((result.Status & MatchTerminatorStatus.Success) > 0)
                {
                    int extra = state.Length - state.MatchPosition;
                    state.SlidingWindow.DequeueRemove(extra);
                    state.CurrentEnumerator = null;
                }

                return state;
            }
            finally
            {
#if (DEBUG) 
                MatchStateDebugLog(state, "Exit"); 
#endif
            }
        }
        #endregion  
    }
}
