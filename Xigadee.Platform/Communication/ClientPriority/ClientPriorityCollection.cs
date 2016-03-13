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
    public class ClientPriorityCollection:StatisticsBase<ClientPriorityStatistics>
    {
        #region Declarations
        private Dictionary<Guid, ClientPriorityHolder> mListenerClients;
        private Guid[] mListenerPollChain;
        private long mListenerCurrent;
        private int mActive;
        private bool mIsClosed;

        private readonly long mIteration;
        private readonly ClientPollSlotAllocationAlgorithm mClientPriorityAlgorithm;
        #endregion
        #region Constructor
        /// <summary>
        /// This constructor build up the poll collection and sets the correct poll priority.
        /// </summary>
        /// <param name="listenerClients">The current list of listener clients.</param>
        public ClientPriorityCollection(Dictionary<Guid, ClientPriorityHolder> listenerClients, ClientPollSlotAllocationAlgorithm clientPriorityAlgorithm, long iterationId)
        {
            mClientPriorityAlgorithm = clientPriorityAlgorithm;
            mIteration = iterationId;

            Created = DateTime.UtcNow;

            if (listenerClients == null)
                throw new ArgumentNullException("ClientPriorityCollection - listenerClients");

            mListenerClients = listenerClients;

            mListenerPollChain = listenerClients
                .Where((c) => c.Value.IsActive)
                .OrderByDescending((c) => c.Value.CalculatePriority())
                .Select((c) => c.Key)
                .ToArray();

            mListenerCurrent = -1;

            mListenerClients.ForEach((c) => c.Value.CapacityReset());
        }
        #endregion

        #region Created
        /// <summary>
        /// This is the time that the collection was created. It is used for reporting a debug purposes.
        /// </summary>
        public DateTime? Created { get; private set; }
        #endregion

        #region TakeNext(int available, Action<int> recoverSlots, out HolderSlotContext context)
        /// <summary>
        /// This method returns the next client in the poll chain if one is available.
        /// </summary>
        /// <param name="available">The number of available slots.</param>
        /// <param name="context">The slot context.</param>
        /// <returns>Returns true if a slot has been reserved.</returns>
        public bool TakeNext(int available, Action<int> recoverSlots, out HolderSlotContext context)
        {
            if (recoverSlots == null)
                throw new ArgumentNullException("ClientPriorityCollection/TakeNext recoverSlots cannot be null");

            context = null;
            if (available <= 0 || mIsClosed)
                return false;

            int loopCount = -1;

            while (++loopCount < mListenerPollChain.Length)
            {
                long slot = Interlocked.Increment(ref mListenerCurrent) % mListenerPollChain.Length;
                Guid clientId = mListenerPollChain[slot];

                ClientPriorityHolder holder;
                if (!mListenerClients.TryGetValue(clientId, out holder))
                    continue;

                //If the holder is already polling then skip
                if (holder.IsReserved)
                    continue;
                //If the holder is infrequently returning data then it will start to 
                //skip poll slots to speed up retrieval for the active slots
                if (holder.ShouldSkip())
                    continue;

                int taken;
                long holderSlotId;
                if (!holder.Reserve(available, out taken, out holderSlotId))
                    continue;

                Interlocked.Increment(ref mActive);

                string clientName;
                if (!string.IsNullOrEmpty(holder.Client.MappingChannelId))
                    clientName= holder.Client.Name + "-" + holder.Client.MappingChannelId;
                else
                    clientName = holder.Client.Name;

                context = new HolderSlotContext(holder.Id, holderSlotId, clientName, taken, holder
                    , complete: (c) =>
                    {
                        recoverSlots(c.Reserved);
                        Interlocked.Decrement(ref mActive);
                        if (c.Actual.HasValue && c.Actual >= c.Reserved)
                        {
                            if (slot < (mListenerCurrent % mListenerPollChain.Length))
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
            mIsClosed = true;
        }
        #endregion

        protected override void StatisticsRecalculate()
        {
            base.StatisticsRecalculate();

            try
            {
                mStatistics.Name = string.Format("Iteration {0} @ {1} {2}", mIteration, Created, mIsClosed?"Closed":"");

                var data = new List<ClientPriorityHolderStatistics>();
                mListenerPollChain.ForIndex((i,g) =>
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
    }
}
