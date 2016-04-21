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
    /// <summary>
    /// This class primarily contains the active requests currently passing through the Microservice.
    /// This class can be used by other components that need to track active requests and processes.
    /// </summary>
    public class RequestTrackerContainer<R>
        where R: RequestTracker, new()
    {
        #region Declarations
        /// <summary>
        /// This is the default processing time for a job.
        /// </summary>
        protected TimeSpan mMaxProcessingTime;
        /// <summary>
        /// This is the action to process a specific imcoming message.
        /// </summary>
        protected Action<string, TransmissionPayload, R> mProcessMessage;
        /// <summary>
        /// This function can be used to set the key for the request.
        /// </summary>
        protected Func<IService, TransmissionPayload, string> mKeyMaker;
        /// <summary>
        /// This is the collection of inplay messages.
        /// </summary>
        protected ConcurrentDictionary<string, R> mInPlayRequests;
        /// <summary>
        /// A pre-created empty list for returning when nothing to be cancelled
        /// </summary>
        private readonly List<R> mEmptyRequestPayloadList = new List<R>(); 
        #endregion
        #region Constructor
        /// <summary>
        /// This is the public constructor that is used to call the dispatcher ProcessInternalMessageHandlers.
        /// </summary>
        /// <param name="ProcessInternalMessageHandlers">The dispatcher method.</param>
        public RequestTrackerContainer(
            Action<string, TransmissionPayload, R> ProcessMessage
            , TimeSpan? maxProcessingTime = null
            , Func<IService,TransmissionPayload,string> keyMaker=null)
        {
            mProcessMessage = ProcessMessage;
            mKeyMaker = keyMaker;
            mMaxProcessingTime = maxProcessingTime ?? TimeSpan.FromSeconds(30);
            mInPlayRequests = new ConcurrentDictionary<string, R>();
        }
        #endregion

        #region HolderCreate(IService caller, TransmissionPayload payload)
        /// <summary>
        /// This method creates the holder that contains the message currently being processed or queued for processing.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="payload">The payload to process.</param>
        /// <returns>Returns a new holder of the specific type.</returns>
        protected virtual R HolderCreate(IService caller, TransmissionPayload payload)
        {
            //Get a new id.
            string id = Key(caller, payload);

            TimeSpan processingTime = payload.MaxProcessingTime.HasValue ? payload.MaxProcessingTime.Value : mMaxProcessingTime;

            //Create and register the request holder.
            var holder = new R()
            {
                Id = id,
                Payload = payload,
                TTL = processingTime,
            };

            return holder;
        }
        #endregion

        #region ProcessMessage(IService caller, TransmissionPayload requestPayload)
        /// <summary>
        /// This method marshalls the incoming requests from the Initiators.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <param name="message">The message to process.</param>
        public virtual R ProcessMessage(IService caller, TransmissionPayload payload)
        {
            //Create and register the request holder.
            var holder = HolderCreate(caller, payload);

            Register(holder);

            ProcessMessageInternal(holder);

            return holder;
        }
        #endregion
        #region ProcessMessageInternal(R holder)
        /// <summary>
        /// This method sends the job to the underlying infrastructure.
        /// </summary>
        /// <param name="holder">The holder to transmit.</param>
        protected virtual void ProcessMessageInternal(R holder)
        {
            mProcessMessage?.Invoke(holder.Id, holder.Payload, holder);
        } 
        #endregion

        #region Register(R holder)
        /// <summary>
        /// This method register a holder with the id specified in the object.
        /// </summary>
        /// <param name="holder">The holder to to add.</param>
        protected virtual void Register(R holder)
        {
            if (!mInPlayRequests.TryAdd(holder.Id, holder))
                throw new Exception("Duplicate key");
        }
        #endregion

        #region Remove(string id)
        /// <summary>
        /// This method removes a request from the inplay collection.
        /// </summary>
        /// <param name="id">The request id.</param>
        /// <returns>Returns true if successful.</returns>
        public virtual bool Remove(string id)
        {
            R holder;
            return Remove(id, out holder);
        }
        #endregion
        #region Remove(string id, out R holder)
        /// <summary>
        /// This method removes a request from the inplay collection.
        /// </summary>
        /// <param name="id">The request id.</param>
        /// <param name="holder">An output containing the holder object.</param>
        /// <returns>Returns true if successful.</returns>
        public virtual bool Remove(string id, out R holder)
        {
            return mInPlayRequests.TryRemove(id, out holder);
        } 
        #endregion

        #region Key(ServiceBase caller, TransmissionPayload payload)
        /// <summary>
        /// This method formats the key used to hold the priority processes.
        /// </summary>
        /// <param name="caller">The calling object.</param>
        /// <returns>Returns a formatted string containing both parts.</returns>
        protected virtual string Key(IService caller, TransmissionPayload payload)
        {
            if (mKeyMaker != null)
                return mKeyMaker(caller, payload);

            string type = caller == null ? "" : caller.GetType().Name;
            return string.Format("{0}|{1}", type, Guid.NewGuid().ToString("N")).ToLowerInvariant();
        }
        #endregion

        public virtual int ProcessTimeoutAll(Action<R> timeout)
        {
            if (mInPlayRequests.IsEmpty)
                return 0;

            var results = mInPlayRequests.ToList();
            return ProcessTimeout(results, timeout).Count;
        }

        #region ProcessTimeout(Action<R> timeout)
        /// <summary>
        /// This method is used to signal requests that has exceeded their wait time.
        /// </summary>
        public virtual List<R> ProcessTimeout(Action<R> timeout)
        {
            if (mInPlayRequests.IsEmpty)
                return mEmptyRequestPayloadList;

            var results = mInPlayRequests.Where(i => i.Value.HasExpired).ToList();
            if (results.Count > 0)
                return ProcessTimeout(results, timeout);

            return mEmptyRequestPayloadList;
        }
        #endregion

        private List<R> ProcessTimeout(IList<KeyValuePair<string, R>> results, Action<R> timeout = null)
        {
            List<R> timedOut = new List<R>(results.Count);
            foreach (var key in results)
            {
                //Check that the object has not be removed by another process.
                R holder;
                if (!Remove(key.Key, out holder))
                    continue;

                if (timeout != null)
                {
                    timeout(holder);
                    timedOut.Add(holder);
                }
            }

            return timedOut;
        }
    }
}
