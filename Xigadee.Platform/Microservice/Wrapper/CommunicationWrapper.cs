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
    internal class CommunicationWrapper: WrapperBase, IMicroserviceCommunication
    {
        /// <summary>
        /// This container holds the communication components.
        /// </summary>
        protected CommunicationContainer mCommunication;

        public CommunicationWrapper(CommunicationContainer communication, Func<ServiceStatus> getStatus) : base(getStatus)
        {
            mCommunication = communication;
        }

        //Communication
        #region RegisterListener(IListener listener)
        /// <summary>
        /// This method regsiters a listener.
        /// </summary>
        /// <typeparam name="C">The listener channelId that implements IListener</typeparam>
        public virtual IListener RegisterListener(IListener listener)
        {
            ValidateServiceNotStarted();
            mCommunication.ListenerAdd(listener);
            return listener;
        }
        #endregion
        #region RegisterSender(ISender sender)
        /// <summary>
        /// This method registers a sender that implements ISender.
        /// </summary>
        /// <typeparam name="C"></typeparam>
        public virtual ISender RegisterSender(ISender sender)
        {
            ValidateServiceNotStarted();
            mCommunication.SenderAdd(sender);
            return sender;
        }
        #endregion
        //Channels
        #region RegisterChannel(Channel logger)
        /// <summary>
        /// This method can be used to manually register an Collector?.
        /// </summary>
        public virtual Channel RegisterChannel(Channel channel)
        {
            ValidateServiceNotStarted();
            mCommunication.Add(channel);
            return channel;
        }
        #endregion
        #region Channels
        /// <summary>
        /// This is a list of jobs currently register in the service.
        /// </summary>
        public virtual IEnumerable<Channel> Channels
        {
            get
            {
                if (mCommunication.Channels == null)
                    yield break;
                else
                    foreach (var channel in mCommunication.Channels)
                        yield return channel;
            }
        }
        #endregion
    }
}
