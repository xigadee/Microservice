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
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    //Components
    public partial class Microservice
    {
        //Communication
        #region RegisterListener(IListener listener)
        /// <summary>
        /// This method regsiters a listener.
        /// </summary>
        /// <typeparam name="C">The listener channelId that implements IListener</typeparam>
        public virtual IListener RegisterListener(IListener listener)
        {
            ValidateServiceNotStarted();
            mCommunication.ListenerAdd(listener, false);
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
        #region RegisterDeadLetterListener(IListener deadLetter)
        /// <summary>
        /// This allows you to manually register a DeadLetterListener
        /// </summary>
        public virtual IListener RegisterDeadLetterListener(IListener deadLetter)
        {
            ValidateServiceNotStarted();
            mCommunication.ListenerAdd(deadLetter, true);
            return deadLetter;
        }
        #endregion
        #region RegisterChannel(Channel logger)
        /// <summary>
        /// This method can be used to manually register an Logger.
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

        //Command
        #region RegisterCommand(IMessageHandler command)
        /// <summary>
        /// This method allows you to manually register a job.
        /// </summary>
        public virtual ICommand RegisterCommand(ICommand command)
        {
            ValidateServiceNotStarted();

            return mCommands.Add(command);
        }
        #endregion
        #region Commands
        /// <summary>
        /// This is a list of jobs currently register in the service.
        /// </summary>
        public virtual IEnumerable<ICommand> Commands
        {
            get
            {
                if (mCommands == null)
                    yield break;
                else
                    foreach (var command in mCommands.Commands.Where((c) => c is ICommand).Cast<ICommand>())
                        yield return command;
            }
        }
        #endregion
        #region SharedServices
        /// <summary>
        /// This collection holds the shared services for the Microservice.
        /// </summary>
        public ISharedService SharedServices { get { return mCommands.SharedServices; } }
        #endregion

        //Collector
        #region RegisterDataCollector(IDataCollectorComponent collector)
        /// <summary>
        /// This method is used to register a collector.
        /// </summary>
        /// <param name="collector">The collector component.</param>
        /// <returns>Returns the collector passed through the registration.</returns>
        public IDataCollectorComponent RegisterDataCollector(IDataCollectorComponent collector)
        {
            ValidateServiceNotStarted();
            mDataCollection.Add(collector);
            return collector;
        }
        #endregion
        #region RegisterEventSource(IEventSourceComponent eventSource)
        /// <summary>
        /// This method can be used to manually register an EventSource.
        /// </summary>
        public virtual IEventSourceComponent RegisterEventSource(IEventSourceComponent eventSource)
        {
            ValidateServiceNotStarted();
            mDataCollection.Add(eventSource);
            return eventSource;
        }
        #endregion
        #region RegisterTelemetry(ITelemetry telemetry)
        /// <summary>
        /// This method can be used to manually register a telemetry logger.
        /// </summary>
        public virtual ITelemetry RegisterTelemetry(ITelemetry telemetry)
        {
            ValidateServiceNotStarted();
            mDataCollection.Add(telemetry);
            return telemetry;
        }
        #endregion
        #region RegisterLogger(ILogger logger)
        /// <summary>
        /// This method can be used to manually register an Logger.
        /// </summary>
        public virtual ILogger RegisterLogger(ILogger logger)
        {
            ValidateServiceNotStarted();
            mDataCollection.Add(logger);
            return logger;
        }
        #endregion
        #region RegisterBoundaryLogger(IBoundaryLoggerComponent logger)
        /// <summary>
        /// This method can be used to manually register an Logger.
        /// </summary>
        public virtual IBoundaryLoggerComponent RegisterBoundaryLogger(IBoundaryLoggerComponent logger)
        {
            ValidateServiceNotStarted();
            mDataCollection.Add(logger);
            return logger;
        }
        #endregion

        //Serializer
        #region RegisterPayloadSerializer(IPayloadSerializer serializer)
        /// <summary>
        /// This method allows you to manually register a requestPayload serializer.
        /// </summary>
        /// <typeparam name="C">The requestPayload serializer channelId.</typeparam>
        public virtual IPayloadSerializer RegisterPayloadSerializer(IPayloadSerializer serializer)
        {
            ValidateServiceNotStarted();
            mPayloadSerializers.Add(serializer.Identifier, serializer);
            return serializer;
        }
        #endregion
        #region ClearPayloadSerializers()
        /// <summary>
        /// This method clears the payload serializers. This may be used as by default, we add the JSON based serializer to Microservice.
        /// </summary>
        public virtual void ClearPayloadSerializers()
        {
            ValidateServiceNotStarted();
            mPayloadSerializers.Clear();
        } 
        #endregion

        //Populate
        #region PopulateComponents()
        /// <summary>
        /// This method is used to populate the items in the ServiceBusContainer prior 
        /// to the index commands.
        /// </summary>
        protected virtual void PopulateComponents()
        {
            mSerializer = InitialiseSerializationContainer(mPayloadSerializers.Values);
        }
        #endregion            
    }
}