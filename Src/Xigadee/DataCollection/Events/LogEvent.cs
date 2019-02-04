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
using System.Diagnostics;

namespace Xigadee
{
    /// <summary>
    /// This is the root class for logging events for the Microservice framework.
    /// </summary>
    [DebuggerDisplay("{Level} {Category} ")]
    public class LogEvent: EventBase
    {
        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEvent"/> class.
        /// </summary>
        protected LogEvent() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEvent"/> class.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public LogEvent(Exception ex) : this(LoggingLevel.Error, null, null, ex)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEvent"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The exception.</param>
        public LogEvent(string message, Exception ex) : this(LoggingLevel.Error, message, null, ex)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEvent"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public LogEvent(string message) : this(LoggingLevel.Info, message, null, null)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEvent"/> class.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        public LogEvent(LoggingLevel level, string message) : this(level, message, null, null)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEvent"/> class.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        public LogEvent(LoggingLevel level, string message, string category) : this(level, message, category, null)
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEvent"/> class.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <param name="message">The message.</param>
        /// <param name="category">The category.</param>
        /// <param name="ex">The exception.</param>
        public LogEvent(LoggingLevel level, string message, string category, Exception ex)
        {
            Level = level;
            Message = message;
            Category = category;
            Ex = ex;

            AdditionalData = new Dictionary<string, string>();
        }
        #endregion        
        /// <summary>
        /// Gets or sets the logging level.
        /// </summary>
        public virtual LoggingLevel Level { get; set; }
        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        public virtual string Message { get; set; }
        /// <summary>
        /// Gets or sets the logging category.
        /// </summary>
        public virtual string Category { get; set; }
        /// <summary>
        /// Gets or sets the exception associated with the log event..
        /// </summary>
        public virtual Exception Ex { get; set; }
        /// <summary>
        /// Gets the additional data collection associated with the event..
        /// </summary>
        public virtual Dictionary<string, string> AdditionalData { get; }
    }
}
