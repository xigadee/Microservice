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
    public static partial class CorePipelineExtensions
    {
        /// <summary>
        /// This method can be used to call out the pipeline flow to an external method.
        /// </summary>
        /// <param name="pipe">The pipeline.</param>
        /// <param name="condition">A boolean condition for the call out.</param>
        /// <param name="whenTrue">The condition true action.</param>
        /// <param name="whenFalse">The condition false action.</param>
        /// <returns>Returns the original Pipeline.</returns>
        public static P Condition<P>(this P pipe, Func<IEnvironmentConfiguration, bool> condition
            , Action<P> whenTrue = null
            , Action<P> whenFalse = null
            )
            where P : IPipelineBase
        {
            if (condition == null)
                throw new ArgumentNullException("condition", "condition cannot be null for this pipeline extension.");

            bool success = condition(pipe.ToConfiguration());

            if (success)
                whenTrue?.Invoke(pipe);
            else
                whenFalse?.Invoke(pipe);

            return pipe;
        }


    }
}
