using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Xigadee
{
    /// <summary>
    /// This filter is used to stop requests from being processed when the Microservice has not completed its start logic.
    /// </summary>
    public class MicroserviceUnavailableFilter: IAsyncAuthorizationFilter
    {
        /// <summary>Synchronisation Lock</summary>
        protected object mSyncLock = new object();

        /// <summary>
        /// Reset event to signal that the microservice's reset event has been
        /// </summary>
        protected ManualResetEvent mStatusChangeResetEvent = new ManualResetEvent(false);

        /// <summary>
        /// Indicates whether this filter has subscribed already to the microservice status changed event
        /// </summary>
        protected bool mIsSubscribedToStatusEvent;


        /// <summary>This is the default constructor.</summary>
        /// <param name="retryInSeconds">The default retry time in seconds.</param>
        /// <param name="waitToStartSeconds">The number of seconds to wait prior to returning 503 starting</param>
        public MicroserviceUnavailableFilter(IMicroservice ms, int retryInSeconds = 10, int waitToStartSeconds = 0)
        {
            Microservice = ms;
            RetryInSeconds = retryInSeconds;
            WaitToStartSeconds = waitToStartSeconds;
        }

        /// <summary>
        /// Gets the microservice.
        /// </summary>
        protected IMicroservice Microservice { get; }

        /// <summary>The default retry time in seconds.</summary>
        public int? RetryInSeconds { get; set; }

        /// <summary>
        /// The number of seconds to wait for the microservice to start before returning 503.
        /// </summary>
        public int? WaitToStartSeconds { get; set; }


        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var ms = context.ToMicroservice();

            var status = ms?.Status ?? ServiceStatus.Faulted;

            throw new NotImplementedException();
        }



        ///// <summary>
        ///// This method is called when the incoming request is executed.
        ///// </summary>
        ///// <param name="actionContext">The action context.</param>
        ///// <param name="cancellationToken">The cancellation token.</param>
        ///// <param name="continuation">The continuation function.</param>
        ///// <returns>Returns a response message based on the current Microservice status.</returns>
        //public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(
        //    HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        //{
        //    var ms = actionContext.ToMicroservice();

        //    var status = ms?.Status ?? ServiceStatus.Faulted;

        //    //Ok, the service is running or is running after the defined wait period, so keep going.
        //    if (status == ServiceStatus.Running ||
        //        ((WaitToStartSeconds ?? 0) > 0 &&
        //            await IsMicroserviceRunning(ms, TimeSpan.FromSeconds(WaitToStartSeconds.Value))))
        //    {
        //        return await continuation();
        //    }

        //    //Service has not yet started, so returns a service unavailable response.
        //    var request = actionContext.Request;
        //    HttpResponseMessage response = request.CreateResponse(HttpStatusCode.ServiceUnavailable);
        //    response.ReasonPhrase = $"Status: {status.ToString()}";

        //    if (RetryInSeconds.HasValue)
        //        response.Headers.Add("Retry-After", RetryInSeconds.Value.ToString());

        //    actionContext.Response = response;

        //    return response;
        //}

        /// <summary>
        /// Indicates whether the microservice is running. Will wait for the supplied duration for
        /// the service to come up if a duration has been supplied
        /// </summary>
        /// <param name="microservice"></param>
        /// <param name="waitToStartDuration"></param>
        /// <returns></returns>
        private async Task<bool> IsMicroserviceRunning(IMicroservice microservice, TimeSpan waitToStartDuration)
        {
            if (microservice == null)
                return false;

            lock (mSyncLock)
            {
                if (microservice.Status == ServiceStatus.Running)
                    return true;

                if (!mIsSubscribedToStatusEvent)
                {
                    microservice.StatusChanged += new EventHandler<StatusChangedEventArgs>(Microservice_StatusChanged);
                    mIsSubscribedToStatusEvent = true;
                }
            }

            if (microservice.Status == ServiceStatus.Running)
                return true;

            await mStatusChangeResetEvent.WaitOneAsync(waitToStartDuration);
            return microservice.Status == ServiceStatus.Running;
        }

        /// <summary>
        /// Event handler that indicates the microservice has changed status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Microservice_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e == null || e.StatusNew != ServiceStatus.Running)
                return;

            IMicroservice microservice = sender as IMicroservice;
            if (microservice != null && mIsSubscribedToStatusEvent)
            {
                lock (mSyncLock)
                {
                    if (mIsSubscribedToStatusEvent)
                    {
                        microservice.StatusChanged -= new EventHandler<StatusChangedEventArgs>(Microservice_StatusChanged);
                        mIsSubscribedToStatusEvent = false;
                    }
                }
            }

            mStatusChangeResetEvent.Set();
        }

    }
}
