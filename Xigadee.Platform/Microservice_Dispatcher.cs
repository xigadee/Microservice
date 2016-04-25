#region using

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion
namespace Xigadee
{
    //Dispatcher
    public partial class Microservice
    {
        #region -->Execute(TransmissionPayload requestPayload)
        /// <summary>
        /// This is the core method that messages are sent to to be routed and processed.
        /// You can override this task in your service to help debug the messages that are passing 
        /// though.
        /// </summary>
        /// <param name="requestPayload">The request payload.</param>
        protected virtual async Task Execute(TransmissionPayload requestPayload)
        {
            int timerStart = StatisticsInternal.ActiveIncrement();

            bool isSuccess = false;
            try
            {
                requestPayload.Cancel.ThrowIfCancellationRequested();

                //If there is no message then we cannot continue.
                if (requestPayload == null || requestPayload.Message == null)
                    throw new ArgumentNullException("Payload or Message not present");
                
                //Check whether this message is for external processing only.
                bool externalOnly = (requestPayload.Options & ProcessOptions.RouteInternal) == 0;
                bool internalOnly = (requestPayload.Options & ProcessOptions.RouteExternal) == 0;
              
                //Check that we have not exceeded the maximum transit count.
                IncrementAndVerifyDispatcherTransitCount(requestPayload);

                //Shortcut for external messages
                if (externalOnly)
                {
                    isSuccess = await mCommunication.Send(requestPayload);
                    if (!isSuccess)
                    {
                        OnProcessRequestUnresolved(requestPayload);
                        mLogger.LogPayload(requestPayload, ex: new SenderNotResolvedException(requestPayload));
                    }
                    return;
                }

                var responsePayloads = new List<TransmissionPayload>();

                bool resolveInternal = await mCommands.Execute(requestPayload, responsePayloads);
                //Switch the incoming message to the outgoing collection to be processed
                //by the sender as it has not been processed internally and it is not marked
                //as internal only.
                if (!resolveInternal && internalOnly)
                {
                    //OK, we have an problem. We log this as an error and get out of here
                    mLogger.LogPayload(requestPayload, ex:new MessageHandlerNotResolvedException(requestPayload));
                    OnProcessRequestUnresolved(requestPayload);
                    isSuccess = ConfigurationOptions.UnhandledMessagesIgnore;
                    return;
                }
                else if (!resolveInternal)
                {
                    //Make sure that this doesn't route back in.
                    requestPayload.Options = ProcessOptions.RouteExternal;
                    responsePayloads.Add(requestPayload);
                }

                if (responsePayloads.Count > 0)
                {
                    //Get the payloads that should be processed internally.
                    var internalPayload = responsePayloads
                        .Where((p) => ((p.Options & ProcessOptions.RouteInternal) > 0) && mCommands.Resolve(p))
                        .ToList();

                    //OK, send the payloads off to the Dispatcher for processing.
                    internalPayload.ForEach((p)=>
                    {
                        //Mark internal only to stop any looping.
                        p.Options = ProcessOptions.RouteInternal;
                        mTaskManager.ExecuteOrEnqueue(p,"Dispatcher");
                    });

                    var externalPayload = responsePayloads.Except(internalPayload).ToList();
                    //Process the external payload to their specific destination.
                    await Task.WhenAll(externalPayload.Select((p) => mCommunication.Send(p)));
                }

                //Set the message to success
                isSuccess = true;
            }
            catch (TransmissionPayloadException pyex)
            {
                mLogger.LogPayload(pyex.Payload, ex: pyex, level: LoggingLevel.Warning);
                OnProcessRequestError(pyex.Payload, pyex);
            }
            catch (Exception ex)
            {
                mLogger.LogException($"Unable to process {requestPayload?.Message} after {requestPayload?.Message?.FabricDeliveryCount} attempts", ex);
                OnProcessRequestError(requestPayload, ex);
            }
            finally
            {
                //Signal to the underlying listener that the message can be released.
                if (requestPayload.DispatcherCanSignal)
                    requestPayload.Signal(isSuccess);

                int delta = StatisticsInternal.ActiveDecrement(timerStart);

                //Log the telemtry for the specific message channelId.
                mTelemetry.Log(requestPayload.Message.ToKey(), delta, isSuccess);

                if (isSuccess)
                    mLogger.LogPayload(requestPayload, direction:DispatcherLoggerDirection.Outgoing, timespan: TimeSpan.FromMilliseconds(delta));
                else
                    StatisticsInternal.ErrorIncrement();
            }
        }
        #endregion

        #region IncrementAndVerifyDispatcherTransitCount(TransmissionPayload requestPayload)
        /// <summary>
        /// This method checks that the incoming message has not exceeded the maximum number of hops.
        /// </summary>
        /// <param name="payload">The requestPayload to check.</param>
        private void IncrementAndVerifyDispatcherTransitCount(TransmissionPayload payload)
        {
            //Increase the Dispatcher transit count.
            payload.Message.DispatcherTransitCount = payload.Message.DispatcherTransitCount + 1;
            //Have we exceeded the transit count for the message.
            if (payload.Message.DispatcherTransitCount > ConfigurationOptions.TransitCountMax)
            {
                //this is not supported message - the message handler will be null if previously processed.
                throw new TransitCountExceededException(payload);
            }
        }
        #endregion
    }
}
