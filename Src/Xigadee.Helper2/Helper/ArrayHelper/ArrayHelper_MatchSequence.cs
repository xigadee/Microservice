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
    /// <summary>
    /// This static class provides a number of extension method for array objects.
    /// </summary>
    public static partial class ArrayHelper
    {
        #region MatchSequence<TSource, TMatch>(this IEnumerable<TSource> source, IEnumerable<TMatch> match)
        /// <summary>
        /// This method matches the sequence against the source array.
        /// </summary>
        /// <typeparam name="TSource">The source array type.</typeparam>
        /// <typeparam name="TMatch">The match array type.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="match">The source array.</param>
        /// <returns>The outgoing match state. This will indicate whether the match was successful or partially successful, i.e. there
        /// is a partial match at the end of the array that cannot be fully resolved.</returns>
        public static MatchState<TSource> MatchSequence<TSource, TMatch>(
            this IEnumerable<TSource> source, IEnumerable<TMatch> match)
        {
            Func<TSource, TMatch, bool> validate = (x, y) => x.Equals(y);
            IEnumerator<TSource> sourceEnum = source.GetEnumerator();

            return sourceEnum.MatchSequence(match, validate, null);
        }
        #endregion  
        #region MatchSequence<TSource, TMatch>(this IEnumerable<TSource> source, IEnumerable<TMatch> match, MatchState<TSource> state)
        /// <summary>
        /// This method matches the sequence against the source array.
        /// </summary>
        /// <typeparam name="TSource">The source array type.</typeparam>
        /// <typeparam name="TMatch">The match array type.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="match">The source array.</param>
        /// <param name="state">The current match state. This can be passed in when matching chucks of data from multiple source arrays.</param>
        /// <returns>The outgoing match state. This will indicate whether the match was successful or partially successful, i.e. there
        /// is a partial match at the end of the array that cannot be fully resolved.</returns>
        public static MatchState<TSource> MatchSequence<TSource, TMatch>(
            this IEnumerable<TSource> source, IEnumerable<TMatch> match, MatchState<TSource> state)
        {
            Func<TSource, TMatch, bool> validate = (x, y) => x.Equals(y);
            IEnumerator<TSource> sourceEnum = source.GetEnumerator();

            return sourceEnum.MatchSequence(match, validate, state);
        }
        #endregion  
        #region MatchSequence<TSource, TMatch>(this IEnumerator<TSource> sourceEnum, IEnumerable<TMatch> match, MatchState<TSource> state)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TMatch"></typeparam>
        /// <param name="sourceEnum"></param>
        /// <param name="match"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static MatchState<TSource> MatchSequence<TSource, TMatch>(
            this IEnumerator<TSource> sourceEnum, IEnumerable<TMatch> match, MatchState<TSource> state)
        {
            Func<TSource, TMatch, bool> predicate = (x, y) => x.Equals(y);

            return sourceEnum.MatchSequence(match, predicate, state);
        }
        #endregion  
        #region MatchSequence<TSource, TMatch>(this IEnumerable<TSource> source, IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state)
        /// <summary>
        /// This method matches the sequence against the source array.
        /// </summary>
        /// <typeparam name="TSource">The source array type.</typeparam>
        /// <typeparam name="TMatch">The match array type.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="match">The source array.</param>
        /// <param name="predicate">The prediciate used to match the source and match array elements.</param>
        /// <param name="state">The current match state. This can be passed in when matching chucks of data from multiple source arrays.</param>
        /// <returns>The outgoing match state. This will indicate whether the match was successful or partially successful, i.e. there
        /// is a partial match at the end of the array that cannot be fully resolved.</returns>
        public static MatchState<TSource> MatchSequence<TSource, TMatch>(
            this IEnumerable<TSource> source, IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state)
        {
            IEnumerator<TSource> sourceEnum = source.GetEnumerator();
            return sourceEnum.MatchSequence(match, predicate, state);
        }
        #endregion  
        #region MatchSequence<TSource, TMatch>(this IEnumerator<TSource> sourceEnum, IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TMatch"></typeparam>
        /// <param name="sourceEnum"></param>
        /// <param name="match"></param>
        /// <param name="predicate"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static MatchState<TSource> MatchSequence<TSource, TMatch>(
            this IEnumerator<TSource> sourceEnum, IEnumerable<TMatch> match, Func<TSource, TMatch, bool> predicate, MatchState<TSource> state)
        {
            try
            {
                //Get an internal state to track the match progress, 
                //either create a new one, or get one from the incoming state object.
                MatchState<TSource> stateInternal;
                if (state == null)
                    stateInternal = new MatchState<TSource>();
                else
                    stateInternal = state;

                //Shortcut to skip checking if the match has already been reached in the collection.
                if (stateInternal.IsMatch)
                    return stateInternal;

                //Check whether we are currently matching a block, this can happen when
                //matches are split over chunks of source arrays, such as a byte block passed from a remote socket.
                bool matchActive = stateInternal.CarryOver > 0;
                //Get the match array enumerator at the correct position.
                IEnumerator<TMatch> matchEnum = match.GetEnumeratorAtPosition(stateInternal.CarryOver);

                //Get the current record in the match array.
                TMatch matchItem = matchEnum.Current;

                int posSource = 0;
                int posMatch = stateInternal.CarryOver;

                //Ok, loop through each record in the source and compare against the match array.
                //foreach (TSource item in source)
                while (sourceEnum.MoveNext())
                {
                    TSource item = sourceEnum.Current;
                    //This method logs the data item from the current enumeration if this state is set to record it.
                    stateInternal.DataLog(item);

                    //OK, do we have a match?
                    bool isMatch = predicate(item, matchItem);

                    if (isMatch)
                    {
                        matchActive = true;
                        stateInternal.Length++;

                        stateInternal.SlidingWindow.Enqueue(item);

                        if (matchEnum.MoveNext())
                        {
                            //Add the item to the sliding window for back checking
                            matchItem = matchEnum.Current;
                            posMatch++;
                        }
                        else
                        {
                            matchActive = false;
                            //Ok, we have completed a match.
                            stateInternal.SetMatch(posSource - posMatch);


                            break;
                        }
                    }
                    else
                    {
                        if (matchActive)
                        {


                            if (stateInternal.MultipartMatch)
                            {
                                //We cannot continue as we are only matching part of the records.
                                matchActive = false;
                                stateInternal.Success = false;
                                break;
                            }

                            //We need to check the previous record to see whether there is a partial match in there.
                            //This method will also set the match enumerator to the correct position
                            posMatch = ValidateSlidingWindow<TSource, TMatch>(stateInternal, matchEnum, predicate);
                            stateInternal.Length = posMatch;

                            //OK, no match so reset the match buffer to its default position
                            matchItem = matchEnum.Current;

                            //Set the match to active as we have matched based on the previous set of bytes.
                            matchActive = posMatch > 0;
                        }
                    }
                    posSource++;
                }

                //If the match is still active then we have a partial match and need to pass this out.
                if (matchActive)
                    stateInternal.SetPartialMatch(posSource, posMatch);

                return stateInternal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion  
    }
}
