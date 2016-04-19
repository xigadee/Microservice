#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion
namespace Xigadee
{
    /// <summary>
    /// This class holds the clients and their priority chain.
    /// </summary>
    public class ListenerClientPriorityCollection:StatisticsBase<ClientPriorityStatistics>
    {
        #region Declarations

        private int[] mListenerPollLevels;

        private Dictionary<Guid, ClientPriorityHolder> mListenerClients;

        private Dictionary<int,Guid[]> mListenerPollChain;

        private long mListenerCurrent;

        private int mActive;

        private readonly long mIteration;

        private readonly IListenerClientPollAlgorithm mPollAlgorithm;
        #endregion
        #region Constructor
        /// <summary>
        /// This constructor build up the poll collection and sets the correct poll priority.
        /// </summary>
        /// <param name="listeners"></param>
        /// <param name="deadletterListeners"></param>
        /// <param name="resourceTracker"></param>
        /// <param name="algorithm"></param>
        /// <param name="iterationId"></param>
        public ListenerClientPriorityCollection(
              List<IListener> listeners
            , List<IListener> deadletterListeners
            , IResourceTracker resourceTracker
            , IListenerClientPollAlgorithm algorithm
            , long iterationId
            )
        {
            mPollAlgorithm = algorithm;
            mIteration = iterationId;
            Created = DateTime.UtcNow;

            //Get the list of active clients.
            mListenerClients = new Dictionary<Guid, ClientPriorityHolder>();
            foreach (var listener in listeners.Union(deadletterListeners))
                if (listener.Clients != null)
                    foreach (var client in listener.Clients)
                        mListenerClients.Add(client.Id, new ClientPriorityHolder(resourceTracker, client, listener.MappingChannelId, algorithm));

            //Get the supported levels.
            mListenerPollLevels = mListenerClients
                .Select((c) => c.Value.Client.Priority)
                .Distinct()
                .OrderByDescending((i) => i)
                .ToArray();

            //Finally create a poll chain for each individual priority level
            mListenerPollChain = new Dictionary<int, Guid[]>();

            foreach (int priority in mListenerPollLevels)
            {
                var guids = mListenerClients
                    .Where((c) => c.Value.IsActive && c.Value.Client.Priority == priority)
                    .OrderByDescending((c) => c.Value.CalculatePriority())
                    .Select((c) => c.Key)
                    .ToArray();

                mListenerPollChain.Add(priority, guids);
            }

            mListenerCurrent = -1;

            mListenerClients.ForEach((c) => c.Value.CapacityReset());
        }
        #endregion
        #region StatisticsRecalculate()
        /// <summary>
        /// This method recalculates the client proiority statistics.
        /// </summary>
        protected override void StatisticsRecalculate()
        {
            base.StatisticsRecalculate();

            try
            {
                mStatistics.Name = $"Iteration {mIteration} @ {Created} {(IsClosed ? "Closed" : "")}";

                var data = new List<ClientPriorityHolderStatistics>();
                foreach(int priority in Levels)
                    mListenerPollChain[priority].ForIndex((i, g) =>
                    {
                        var stat = mListenerClients[g].Statistics;
                        stat.Ordinal = i;
                        data.Add(stat);
                    });

                if (Created.HasValue)
                    mStatistics.Created = Created.Value;

                mStatistics.ClientHolders = data;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Created
        /// <summary>
        /// This is the time that the collection was created. It is used for reporting a debug purposes.
        /// </summary>
        public DateTime? Created { get; private set; }
        #endregion

        #region TakeNext(int priority, int available, Action<int> recoverSlots, out HolderSlotContext context)
        /// <summary>
        /// This method returns the next client in the poll chain if one is available.
        /// </summary>
        /// <param name="priority">This is the client priority level.</param>
        /// <param name="available">The number of available slots.</param>
        /// <param name="recoverSlots">This action is called when a poll has completed to recover the reserved slots for other polling clients.</param>
        /// <param name="holder">The slot context.</param>
        /// <returns>Returns true if a slot has been reserved.</returns>
        public bool TakeNext(int priority, int available, Action<int> recoverSlots, out ClientPriorityHolder holder)
        {
            if (recoverSlots == null)
                throw new ArgumentNullException("ClientPriorityCollection/TakeNext recoverSlots cannot be null");

            holder = null;

            if (available <= 0 || IsClosed)
                return false;

            if (!mListenerPollChain.ContainsKey(priority))
                return false;

            int loopCount = -1;

            while (++loopCount < mListenerPollChain[priority].Length)
            {
                holder = null;

                Guid clientId = mListenerPollChain[priority][loopCount];

                //Check whether we can get the client. If not, move to the next.
                if (!mListenerClients.TryGetValue(clientId, out holder))
                    continue;

                //If the holder is already polling then move to the next.
                if (holder.IsReserved)
                    continue;

                //If the holder is infrequently returning data then it will start to 
                //skip poll slots to speed up retrieval for the active slots.
                if (holder.ShouldSkip())
                    continue;

                int taken;
                long holderSlotId;
                if (!holder.Reserve(available, out taken, out holderSlotId))
                    continue;

                Interlocked.Increment(ref mActive);



                context = new HolderSlotContext(priority, holder.Id, holderSlotId, clientName, taken, holder
                    , complete: (c) =>
                    {
                        recoverSlots(c.Reserved);
                        Interlocked.Decrement(ref mActive);
                        if (c.Actual.HasValue && c.Actual >= c.Reserved)
                        {
                            if (slot < (mListenerCurrent % mListenerPollChain[priority].Length))
                                mListenerCurrent = slot;
                        }
                    });

                return true;
            }

            return false;
        }
        #endregion

        #region Close()
        /// <summary>
        /// This method marks the current collection as closed.
        /// </summary>
        public void Close()
        {
            IsClosed = true;
        }
        #endregion
        #region IsClosed
        /// <summary>
        /// This property is set
        /// </summary>
        public bool IsClosed
        {
            get; private set;
        } 
        #endregion

        #region Levels
        /// <summary>
        /// This is the active priority levels for the listener collection.
        /// </summary>
        public int[] Levels
        {
            get
            {
                return mListenerPollLevels;
            }
        }
        #endregion
        #region Count
        /// <summary>
        /// This is the number of active listeners in the poll chain.
        /// </summary>
        public int Count
        {
            get
            {
                return mListenerPollChain.Values.Sum((g) => g.Length);
            }
        } 
        #endregion

        #region Logging
        public void QueueTimeLog(Guid clientId, DateTime? EnqueuedTimeUTC)
        {
            if (mListenerClients.ContainsKey(clientId))
                mListenerClients[clientId].QueueTimeLog(EnqueuedTimeUTC);
        }

        public void ActiveIncrement(Guid clientId)
        {
            if (mListenerClients.ContainsKey(clientId))
                mListenerClients[clientId].ActiveIncrement();
        }

        public void ActiveDecrement(Guid clientId, int TickCount)
        {
            if (mListenerClients.ContainsKey(clientId))
                mListenerClients[clientId].ActiveDecrement(TickCount);
        }

        public void ErrorIncrement(Guid clientId)
        {
            if (mListenerClients.ContainsKey(clientId))
                mListenerClients[clientId].ErrorIncrement();
        }
        #endregion
    }
}
