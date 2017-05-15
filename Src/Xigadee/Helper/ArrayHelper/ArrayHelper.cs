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
    /// This static class provides array based matching logic.
    /// </summary>
    public static partial class ArrayHelper
    {
        #region GetGenericEnumerator<T>(this IEnumerable<T> data)
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerator<T> GetGenericEnumerator<T>(this IEnumerable<T> data)
        {
            IEnumerator<T> enumData = data.GetEnumerator();
            enumData.MoveNext();
            return enumData;
        }
        #endregion  

        #region GetEnumeratorAtPosition<T>(this IEnumerable<T> source, int pos)
        /// <summary>
        /// This method gets a enumerator for the IEnumerable object and moves it forward 
        /// by the required number of places.
        /// </summary>
        /// <typeparam name="T">The enumeration type.</typeparam>
        /// <param name="source">The boject source.</param>
        /// <param name="pos">The number of positions to move forward.</param>
        /// <returns>Returns the object or null if the method has moved passed the end of the collection.</returns>
        public static IEnumerator<T> GetEnumeratorAtPosition<T>(this IEnumerable<T> source, int pos)
        {
            IEnumerator<T> matchEnum = source.GetEnumerator();
            matchEnum.Reset();
            matchEnum.MoveNext();

            //OK, skip through the enumerator until we reach the correct position.
            while (pos > 0)
            {
                if (!matchEnum.MoveNext())
                    return null;
                pos--;
            }

            return matchEnum;
        }
        #endregion

        #region DequeueRemove<T>(this Queue<T> queue, int count)
        /// <summary>
        /// This method will remove and discard the number of items specified from the queue.
        /// </summary>
        /// <typeparam name="T">The queue type.</typeparam>
        /// <param name="queue">The queue.</param>
        /// <param name="count">The number of items to remove.</param>
        public static void DequeueRemove<T>(this Queue<T> queue, int count)
        {
            while (count-- > 0 && queue.Count > 0)
                queue.Dequeue();
        }
        #endregion  

        #region MatchStateDebugLog<TSource, TMatch>(MatchCollectionState<TSource, TMatch> state, string type)
        private static void MatchStateDebugLog<TSource, TMatch>(MatchCollectionState<TSource, TMatch> state, string type)
        {
            if (state.DebugTrace)
                state.DebugTraceCollection.Add(
                    string.Format("{0} <- {1} Q={2}, ({3})"
                    , type
                    , state.DebugTraceRecursion
                    , state.SlidingWindow == null ? "null" : state.SlidingWindow.Count.ToString()
                    , !state.Status.HasValue ? "null" : state.Status.Value.ToString())
                    );

            state.DebugTraceRecursion--;
        }
        #endregion  
    }
}
