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
        /// This method attaches a rewrite rule to the channel pipeline.
        /// </summary>
        /// <typeparam name="P">The channel pipeline type.</typeparam>
        /// <param name="cpipe">The incoming pipeline.</param>
        /// <param name="rewriteRule">The rewrite rule.</param>
        /// <returns>Returns the original pipeline.</returns>
        public static P AttachMessageRedirectRule<P>(this P cpipe, MessageRedirectRule rewriteRule)
            where P: ChannelPipelineBase
        {
            cpipe.Channel.RedirectAdd(rewriteRule);

            return cpipe;
        }



    }  
}
