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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    /// <summary>
    /// This base status class is used for logging job time statistics.
    /// </summary>
    public class StatusBase : LogEvent
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="name">The name to display in the status message.</param>
        /// <param name="batchSize">The batch size to recycle the statistics.</param>
        public StatusBase(string name)
        {
            Name = name??GetType().Name;
            Level = LoggingLevel.Status;
            Created = DateTime.UtcNow;
        }
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="name">The name to display in the status message.</param>
        /// <param name="batchSize">The batch size to recycle the statistics.</param>
        public StatusBase()
        {
            Name = GetType().Name;
            Level = LoggingLevel.Status;
            Created = DateTime.UtcNow;
        }
        #endregion

        #region Name
        /// <summary>
        /// This is the service name
        /// </summary>
        public virtual string Name { get; set; }
        #endregion

        #region ComponentId
        /// <summary>
        /// This is the base component id.
        /// </summary>
        public Guid ComponentId { get; set; } 
        #endregion
        #region Created
        /// <summary>
        /// This is the time the service was created.
        /// </summary>
        public DateTime Created { get; set; } 
        #endregion

        #region Message
        /// <summary>
        /// This is the message logged for simple loggers.
        /// </summary>
        public override string Message
        {
            get
            {
                var now = DateTime.UtcNow;
                return string.Format("{0} @ {1} Uptime {2} "
                    , Name
                    , now
                    , now - Created);
            }
            set
            {
                base.Message = value;
            }
        } 
        #endregion
    }
}
