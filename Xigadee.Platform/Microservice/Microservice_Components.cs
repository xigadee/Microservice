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
        #region SharedServices
        /// <summary>
        /// This collection holds the shared services for the Microservice.
        /// </summary>
        public ISharedService SharedServices { get { return mCommands.SharedServices; } }
        #endregion

        //Comms
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

        //Collector
        #region RegisterCollector(ICollector collector)
        /// <summary>
        /// This method is used to register a collector.
        /// </summary>
        /// <param name="collector">The collectors.</param>
        /// <returns>Returns the collector passed through the registration.</returns>
        public IDataCollector RegisterCollector(IDataCollector collector)
        {
            ValidateServiceNotStarted();
            mDataCollection.Add(collector);
            return collector;
        } 
        #endregion
        //Event Source
        #region RegisterEventSource(IEventSource eventSource)
        /// <summary>
        /// This method can be used to manually register an EventSource.
        /// </summary>
        public virtual IEventSource RegisterEventSource(IEventSource eventSource)
        {
            ValidateServiceNotStarted();
            mDataCollection.Add(eventSource);
            return eventSource;
        }
        #endregion
        //Telemetry
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
        //Logger
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

        //Channel
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
        //Serializer
        #region RegisterPayloadSerializer(IPayloadSerializer serializer)
        /// <summary>
        /// This method allows you to manually register a requestPayload serializer.
        /// </summary>
        /// <typeparam name="C">The requestPayload serializer channelId.</typeparam>
        public virtual IPayloadSerializer RegisterPayloadSerializer(IPayloadSerializer serializer)
        {
            ValidateServiceNotStarted();
            mPayloadSerializers.Add(serializer);
            return serializer;
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
            mSerializer = InitialiseSerializationContainer(mPayloadSerializers);
        }
        #endregion            
    }
}