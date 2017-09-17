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
