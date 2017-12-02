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

using System.Collections.Generic;

namespace Xigadee
{
    /// <summary>
    /// This class contains the default statistics for the command.
    /// </summary>
    /// <seealso cref="Xigadee.MessagingStatistics" />
    public class CommandStatistics: MessagingStatistics
    {
        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        public CommandStatistics() : base()
        {
            MasterJob = new MasterJobStatistics();
        }
        #endregion

        /// <summary>
        /// This is the messaging statistics name.
        /// </summary>
        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
            }
        }
        /// <summary>
        /// Gets or sets the startup priority.
        /// </summary>
        public int StartupPriority { get; set; }
        /// <summary>
        /// Gets or sets the outgoing requests.
        /// </summary>
        public List<string> OutgoingRequests { get; set; }

        /// <summary>
        /// Gets or sets the supported handlers.
        /// </summary>
        public List<ICommandHandlerStatistics> SupportedHandlers { get; set; }
        /// <summary>
        /// Gets or sets the master job statistics.
        /// </summary>
        public MasterJobStatistics MasterJob { get; set; }

    }
}
