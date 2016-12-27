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
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion
namespace Xigadee
{
    /// <summary>
    /// This is the specific helper for Azure ServiceBus Queue.
    /// </summary>
    public static class QueueHelper
    {
        #region QueueDescriptionGet(string cName)
        /// <summary>
        /// This is the default queue description. You should override this if you need a different
        /// defintion.
        /// </summary>
        /// <param name="cName">The queue name.</param>
        /// <returns>Returns a QueueDescription object with the default settings.</returns>
        public static QueueDescription QueueDescriptionGet(string cName
            , TimeSpan? defaultMessageTTL = null
            , TimeSpan? lockDuration = null
            )
        {
            return new QueueDescription(cName)
            {
                  EnableDeadLetteringOnMessageExpiration = true
                , LockDuration = lockDuration ?? TimeSpan.FromMinutes(5)
                , SupportOrdering = true
                , EnableBatchedOperations = true
                , DefaultMessageTimeToLive = defaultMessageTTL ?? TimeSpan.FromDays(7)
                , MaxSizeInMegabytes = 5120

                //, EnablePartitioning = true
            };
        }
        #endregion

        #region QueueFabricInitialize(this NamespaceManager mNamespaceManager, string mConnectionName)
        /// <summary>
        /// This method creates the queue if it doesn't already exist.
        /// </summary>
        public static QueueDescription QueueFabricInitialize(this AzureConnection conn, string name
            , TimeSpan? defaultMessageTTL = null
            , TimeSpan? lockDuration = null
        )
        {
            if (!conn.NamespaceManager.QueueExists(name))
            {
                try
                {
                    return conn.NamespaceManager.CreateQueue(QueueDescriptionGet(name, defaultMessageTTL, lockDuration));
                }
                catch (MessagingEntityAlreadyExistsException)
                {
                    // Another service created it before we did - just retrieve the one it created
                }
            }

            return conn.NamespaceManager.GetQueue(name);
        }
        #endregion
    }
}
