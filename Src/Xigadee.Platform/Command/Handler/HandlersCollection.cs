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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This collection holds the handlers.
    /// </summary>
    public class HandlersCollection: ISupportedMessageTypes
    {
        #region Events
        /// <summary>
        /// The handler change event.
        /// </summary>
        public event EventHandler<SupportedMessagesChangeEventArgs> OnCommandChange; 
        #endregion
        #region Declarations
        /// <summary>
        /// This internal function is used to retrieve the handler collection.
        /// </summary>
        private Func<List<MessageFilterWrapper>> mSupportedMessageTypes;
        #endregion

        #region Constructor
        /// <summary>
        /// This is the default constructor.
        /// </summary>
        /// <param name="supportedMessageTypes">The function pointer to the collection.</param>
        public HandlersCollection(Func<List<MessageFilterWrapper>> supportedMessageTypes)
        {
            mSupportedMessageTypes = supportedMessageTypes;
        } 
        #endregion

        /// <summary>
        /// This message is fired when a command status changes.
        /// </summary>
        /// <param name="messages">The message list to pass to the event.</param>
        public void NotifyChange(List<MessageFilterWrapper> messages)
        {
            try
            {
                OnCommandChange?.Invoke(this, new SupportedMessagesChangeEventArgs(mSupportedMessageTypes()));
            }
            catch
            {
                //We don't throw errors here.
            }
        }

        /// <summary>
        /// This is a list of supported messages.
        /// </summary>
        public List<MessageFilterWrapper> SupportedMessages
        {
            get
            {
                return mSupportedMessageTypes();
            }
        }
    }
}
