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

#region using
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
#endregion
namespace Xigadee
{
    public abstract partial class CommandBase<S, P, H>
    {
        #region ChannelId/ChannelIdAutoSet
        /// <summary>
        /// This is the default listening channel id for incoming requests.
        /// </summary>
        public virtual string ChannelId
        {
            get
            {
                return Policy.ChannelId;
            }
            set
            {
                Policy.ChannelId = value;
            }
        }
        /// <summary>
        /// Specifies whether the channel can be auto-set during configuration
        /// </summary>
        public virtual bool ChannelIdAutoSet
        {
            get { return Policy.ChannelIdAutoSet; }
        }
        #endregion
        #region ResponseChannelId/ResponseChannelIdAutoSet
        /// <summary>
        /// This is the channel used for the response to outgoing messages.
        /// </summary>
        public virtual string ResponseChannelId
        {
            get
            {
                return Policy.ResponseChannelId;
            }
            set
            {
                Policy.ResponseChannelId = value;
            }
        }
        /// <summary>
        /// Specifies whether the response channel can be set during configuration.
        /// </summary>
        public virtual bool ResponseChannelIdAutoSet
        {
            get { return Policy.ResponseChannelIdAutoSet; }
        }
        #endregion
    }
}
